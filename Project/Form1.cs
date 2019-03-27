using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using Kitware.VTK;

namespace Project
{
    public partial class Form1 : Form
    {      
        ReadVTP dataSetOne = new ReadVTP();

        ReadVTP dataSetTwo = new ReadVTP();

        Algorithms algortihmObj = new Algorithms();

        Write writer = new Write();
        List<Vector3d> pointsDS1 = new List<Vector3d>();
        List<Vector3d> pointsDS2 = new List<Vector3d>();

        List<Vector4> tetraPointsDS1 = new List<Vector4>();
        List<Vector3> trianglePointsDS1 = new List<Vector3>();

        List<Vector4> tetraPointsDS2 = new List<Vector4>();
        List<Vector3> trianglePointsDS2 = new List<Vector3>();

        public Dictionary<String, vtkDataArray> scalar_dataArray = new Dictionary<string, vtkDataArray>();
        public List<String> arrayNames = new List<String>();

      
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "vtk files (*.vts,*.vtu, *.vtp)|*.vts; *.vtu;*.vtp|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            //saveFileDialog1.Filter = "vtk files (*.vts,*.vtu)|*.vts; *.vtu|All files (*.*)|*.*";
            exportToolStripMenuItem.Enabled = false;
            registrationToolStripMenuItem.Enabled = false;
            calculationsToolStripMenuItem.Enabled = false;
        }

        private void dataset1ToolStripMenuItem_Click(object sender, EventArgs e)
        {       
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            richTextBox1.Text = filename;
            if (filename.EndsWith("vts"))
            {
                dataSetOne.Read_StructuredGrid_File(filename);
                dataSetOne.isVTS = true;
            }
            else if(filename.EndsWith("vtu"))
            {
                dataSetOne.Read_Unstructured_Grid_File(filename);
                dataSetOne.isVTS = false;
            }
            else
            { 
                dataSetOne.Read_Poly_Data_File(filename);
                dataSetOne.isVTS = false;
            }
            dataSetOne.vertex_data.TryGetValue("vertices", out pointsDS1);

            if (dataSetOne.tetraPoints.Count > 0)
                tetraPointsDS1 = dataSetOne.tetraPoints;
            else if(dataSetOne.trianglePoints.Count > 0)
                trianglePointsDS1 = dataSetOne.trianglePoints;


            scalar_dataArray = dataSetOne.scalar_dataArray;
            arrayNames = dataSetOne.arrayNames;
            richTextBox3.Text +="All points read in correctly! \r";
            exportToolStripMenuItem.Enabled = true;
            registrationToolStripMenuItem.Enabled = true;
            calculationsToolStripMenuItem.Enabled = true;
            if (pointsDS2.Count != 0)
                loadDataToolStripMenuItem.Enabled = false;
        }

        private void dataset2ToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            openFileDialog1.ShowDialog();
            string filename = openFileDialog1.FileName;
            richTextBox2.Text = filename;
            if (filename.EndsWith("vts"))
            {
                dataSetTwo.Read_StructuredGrid_File(filename);
                dataSetTwo.isVTS = true;
            }               
            else
            {
                dataSetTwo.Read_Unstructured_Grid_File(filename);
                dataSetTwo.isVTS = false;
            }
               
            dataSetTwo.vertex_data.TryGetValue("vertices", out pointsDS2);

            if (dataSetTwo.tetraPoints.Count > 0)
                tetraPointsDS2 = dataSetTwo.tetraPoints;
            else if (dataSetTwo.trianglePoints.Count > 0)
                trianglePointsDS2 = dataSetTwo.trianglePoints;

            richTextBox3.Text += "All points read in correctly! \r";
            exportToolStripMenuItem.Enabled = true;
            registrationToolStripMenuItem.Enabled = true;
            calculationsToolStripMenuItem.Enabled = true;
            if (pointsDS1.Count != 0)
                loadDataToolStripMenuItem.Enabled = false;
        }

        private void differenzesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(pointsDS1.Count!= 0 && pointsDS2.Count != 0)
            {
                richTextBox3.Text += "Calculation of deviations started.\r";
                richTextBox3.Update();
                List<int> interpolatedPointArray = algortihmObj.GetNearestPoints(pointsDS1, pointsDS2, dataSetOne.scalar_data, dataSetTwo.scalar_data);

                algortihmObj.CalculateDifferences(pointsDS1, pointsDS2, dataSetOne.scalar_data, dataSetTwo.scalar_data, interpolatedPointArray);
                scalar_dataArray = algortihmObj.scalar_dataArray;
                arrayNames = algortihmObj.arrayNames;

                richTextBox3.Text += "Calculation of deviations finished! Now you should export the data. \r";
                registrationToolStripMenuItem.Enabled = true;
                calculationsToolStripMenuItem.Enabled = true;
                loadDataToolStripMenuItem.Enabled = true;
            }
            else
                richTextBox3.Text += "You need to load two datasets to calculate the deviations! \r";
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isVTS = false;
            int numberOfPoints = 0;
            if (algortihmObj.arrayNames.Count() != 0)
            {
                numberOfPoints = (int)algortihmObj.scalar_dataArray[algortihmObj.arrayNames[0]].GetNumberOfTuples();
            }
            else
                numberOfPoints = pointsDS1.Count();
            
            saveFileDialog1.ShowDialog();
            string outputFileName = saveFileDialog1.FileName;
            if (dataSetOne.isVTS)
            {
                writer.WriteStructuredGrid(outputFileName, pointsDS1, scalar_dataArray, arrayNames, numberOfPoints, dataSetOne.dimensions);
                richTextBox3.Text += "File was saved: " + outputFileName;
                exportToolStripMenuItem.Enabled = false;
            }
            else
            {
                if (dataSetOne.tetraPoints.Count > 0)
                    writer.WriteVTUTetra(outputFileName, pointsDS1, dataSetOne.tetraPoints, scalar_dataArray, arrayNames, numberOfPoints, dataSetOne.tetraPoints.Count());
                else if (dataSetOne.trianglePoints.Count > 0)
                    writer.WriteVTUTriangle(outputFileName, pointsDS1, dataSetOne.trianglePoints, scalar_dataArray, arrayNames, numberOfPoints, dataSetOne.trianglePoints.Count());

                richTextBox3.Text += "File was saved: " + outputFileName;
                exportToolStripMenuItem.Enabled = false;
            }

        }
       
       
        private void registrationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (pointsDS1.Count != 0 && pointsDS2.Count != 0)
            {
                richTextBox3.Text += "Begin registration of the two data sets. This can take 5-10 minutes!\r";
                richTextBox3.Update();
                // pointsDS1 will be registrated to pointsDS2
                List<Vector3d> pointsAllreadyRegistrated = algortihmObj.ICPCalculation(pointsDS1, pointsDS2);
                pointsDS1 = pointsAllreadyRegistrated;
                richTextBox3.Text += "Registration done! You can export the registrated data set now or calculate the deviation of the two data sets!\r";
                lambdaKriteriumToolStripMenuItem.Enabled = false;
                loadDataToolStripMenuItem.Enabled = false;
                richTextBox3.Text += algortihmObj.transMatrix[0] + "\r" + algortihmObj.transMatrix[1] + "\r" + algortihmObj.transMatrix[2] + "\r" + algortihmObj.transMatrix[3] + "\r";

            }
        }

        private void dataset1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataSetOne.isVTS)
            {
                richTextBox3.Text += "Calculation of Lambda2 criterion started.\r";
                richTextBox3.Update();
                algortihmObj.CalculateLambdaCriterionRegularGrids(pointsDS1, dataSetOne.scalar_data, dataSetOne.arrayNames, dataSetOne.dimensions);
                scalar_dataArray = algortihmObj.scalar_dataArray;
                arrayNames = algortihmObj.arrayNames;
                richTextBox3.Text += "Lambda2 criterion was calculated! \r";
            }
            else
            {
                richTextBox3.Text += "Calculation of Lambda2 criterion started.\r";
                richTextBox3.Update();
                algortihmObj.CalculateLambda2CriterionIrregularGrids(pointsDS1, dataSetOne.scalar_data, dataSetOne.arrayNames, dataSetOne.tetraPoints);
                scalar_dataArray = algortihmObj.scalar_dataArray;
                arrayNames = algortihmObj.arrayNames;

                richTextBox3.Text += "Lambda2 criterion was calculated! \r";
            }
        }
    }
}
