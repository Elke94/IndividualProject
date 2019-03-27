using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kitware.VTK;
using OpenTK;
using System.Windows.Forms;

namespace Project
{
    class ReadVTP
    {
        public bool isVTS;
        private bool isTetra = true;

        public Dictionary<String, List<Vector3d>> vertex_data = new Dictionary<string, List<Vector3d>>();
        public Dictionary<String, List<float>> scalar_data = new Dictionary<string, List<float>>();
        public Dictionary<String, List<Vector3>> vector_data = new Dictionary<string, List<Vector3>>();
        public Dictionary<String, vtkDataArray> scalar_dataArray = new Dictionary<string, vtkDataArray>();
        
        public List<Vector4> tetraPoints = new List<Vector4>();
        public List<Vector3> trianglePoints = new List<Vector3>();
        public List<String> arrayNames = new List<String>();
        public int[] dimensions = new int[3];

        public vtkCellArray cells = vtkCellArray.New();
        public List<Vector3> triangleList = new List<Vector3>();
        public bool Read_Poly_Data_File(string filename)
        {
            //Initalize VTK Reader
            vtkXMLPolyDataReader reader = new vtkXMLPolyDataReader();
            
            reader.SetFileName(filename);

            reader.Update();

            vtkPolyData polydata = reader.GetOutput();

            if (polydata == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Poly data Input");

                return false;
            }

            // Read Point Coordinates
            int numPoints = (int)polydata.GetNumberOfPoints();

            List<Vector3d> point_dat = new List<Vector3d>();
            
            if (numPoints != 0)
            {
                double[] pt;

                    for (int i = 0; i < numPoints; i++)
                    {
                        pt = polydata.GetPoint(i);

                        point_dat.Add(new Vector3d((float)pt[0], (float)pt[1], (float)pt[2]));
                    }
                if (this.vertex_data.ContainsKey("vertices"))
                {
                    this.vertex_data["vertices"] = point_dat;
                }
                else
                {
                    this.vertex_data.Add("vertices", point_dat);
                }
                Console.WriteLine("All points read in correctly!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------No Points existent");
            }

            // Read Point Indices
            int numpolydatacells = (int)polydata.GetNumberOfCells();

            vtkCell polydataCell;
            vtkIdList pts;

            if (numpolydatacells != 0)
            {
                int counter = 0;
                cells.SetNumberOfCells(numpolydatacells);
                
                for (int i = 0; i < numpolydatacells; i++)
                {
                    polydataCell = polydata.GetCell(i);
                    
                    int numCellPoints = (int)polydataCell.GetNumberOfPoints();
                    cells.InsertNextCell(polydataCell);

                    Vector3 trianglePoints = new Vector3();
                    if (numCellPoints == 3)
                    {
                        pts = polydataCell.GetPointIds();

                        int one  = (int)pts.GetId(0);
                        int two = (int)pts.GetId(1);
                        int three = (int)pts.GetId(2);
                        //this.Get_Triangle(counter, pts);
                        trianglePoints = new Vector3(one, two, three);
                        counter++;
                    }
                    triangleList.Add(trianglePoints);
                
                }
                
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------No Triangles existent");
            }

            // Read point data
            vtkPointData pointData = polydata.GetPointData();

            // Load point attributes
            this.Load_Point_Attributes(pointData);

            return true;
        }
        
        public void Read_MultiblockData_File(string filename)
        {
            vtkXMLMultiBlockDataReader reader = new vtkXMLMultiBlockDataReader();
            reader.SetFileName(filename);

            reader.Update();

            vtkMultiBlockDataSet data = (vtkMultiBlockDataSet)reader.GetOutput();
            
            vtkInformation vtkInformation = data.GetMetaData(0);
            
            uint numBlocks = data.GetNumberOfBlocks();
            if (numBlocks == 1)
            {            

                vtkStructuredGrid structuredGrid = (vtkStructuredGrid)data.GetBlock(0);
                dimensions = structuredGrid.GetDimensions();

                vtkPoints points = structuredGrid.GetPoints();
                
                int numPoints = (int)structuredGrid.GetNumberOfPoints();
                List<Vector3d> point_dat = new List<Vector3d>();
                if (numPoints != 0)
                {
                    // Read Point Data
                    double[] pt;
                    for (int i = 0; i < numPoints; i++)
                    {
                        pt = points.GetPoint(i);
                        point_dat.Add(new Vector3d((float)pt[0], (float)pt[1], (float)pt[2]));
                    }

                    if (this.vertex_data.ContainsKey("vertices"))
                    {
                        this.vertex_data["vertices"] = point_dat;
                    }
                    else
                    {
                        this.vertex_data.Add("vertices", point_dat);
                    }
                    Console.WriteLine("All points read in correctly!");
                }
                vtkPointData scalarValues = (vtkPointData)data.GetBlock(0).GetAttributes(0);
                // Load point attributes
                this.Load_Point_Attributes(scalarValues);
            }

        }

        public void Read_StructuredGrid_File(string filename)
        {
            vtkXMLStructuredGridReader reader = new vtkXMLStructuredGridReader();
            reader.SetFileName(filename);

            reader.Update();

            vtkStructuredGrid structuredGrid = reader.GetOutput();
            

            
            
                dimensions = structuredGrid.GetDimensions();

                vtkPoints points = structuredGrid.GetPoints();

                int numPoints = (int)structuredGrid.GetNumberOfPoints();
                List<Vector3d> point_dat = new List<Vector3d>();
                if (numPoints != 0)
                {
                    // Read Point Data
                    double[] pt;
                    for (int i = 0; i < numPoints; i++)
                    {
                        pt = points.GetPoint(i);
                        point_dat.Add(new Vector3d((float)pt[0], (float)pt[1], (float)pt[2]));
                    }

                    if (this.vertex_data.ContainsKey("vertices"))
                    {
                        this.vertex_data["vertices"] = point_dat;
                    }
                    else
                    {
                        this.vertex_data.Add("vertices", point_dat);
                    }
                    Console.WriteLine("All points read in correctly!");                
                }
                vtkPointData scalarValues = structuredGrid.GetPointData();
                // Load point attributes
                this.Load_Point_Attributes(scalarValues);
            

        }

        public void Read_Unstructured_Grid_File(string filename)
        {
            // Initalize VTK Reader
            vtkXMLUnstructuredGridReader reader = new vtkXMLUnstructuredGridReader();

            reader.SetFileName(filename);

            reader.Update();
            
            vtkUnstructuredGrid grid = reader.GetOutput();       

            vtkCellArray data = grid.GetCells();
            vtkIdList idList = vtkIdList.New();
            int numCells = (int)data.GetNumberOfCells();

            if (numCells != 0)
            {
                if (grid.GetCellType(0) == 10)
                {
                    isTetra = true;
                    Console.WriteLine("Celltype is tetra");
                }
                else if (grid.GetCellType(0) == 5)
                {
                    isTetra = false;
                    Console.WriteLine("Celltype is triangle");
                }
                else
                    Console.WriteLine("No valid celltype");
            
                for (int i = 0; i < numCells; i++)
                {
                    long cellTypeID = grid.GetCellType(i);
                    // alle punkte durchlaufen und in eine Variable für jeden Punkt die anderen drei Punkte speichern
                    if (isTetra)
                    {
                        Vector4 tetraPoint = new Vector4();

                        grid.GetCellPoints(i, idList);
                        // ueber alle vier punkte iterieren und diese in extra liste für jeden Punkt speichern
                        for (int j = 0; j < 4; j++)
                        {
                            tetraPoint[j] = idList.GetId(j);
                        }

                        tetraPoints.Add(tetraPoint);
                    }
                    else if(!isTetra)
                    {

                        Vector3 triangle = new Vector3();

                        grid.GetCellPoints(i, idList);
                        // ueber alle drei punkte iterieren und diese in extra liste für jeden Punkt speichern                       
                        for (int j = 0; j < 3; j++)
                        {
                            triangle[j] = idList.GetId(j);
                        }
                        trianglePoints.Add(triangle);
                    }
                }  
            }

            // Read Point Coordinates
            vtkPoints points = grid.GetPoints();
            
            int numPoints = (int)points.GetNumberOfPoints();

            List<Vector3d> point_dat = new List<Vector3d>();

            if (numPoints != 0)
            {
                // Read Point Data
                double[] pt;
                for (int i = 0; i < numPoints; i++)
                {
                    pt = points.GetPoint(i);
                    point_dat.Add(new Vector3d((float)pt[0], (float)pt[1], (float)pt[2]));
                }

                if (this.vertex_data.ContainsKey("vertices"))
                {
                    this.vertex_data["vertices"] = point_dat;
                }
                else
                {
                    this.vertex_data.Add("vertices", point_dat);
                }
                Console.WriteLine("All points read in correctly!");

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------No Points existent");
            }

            vtkPointData pointData = grid.GetPointData();
            
            // Load point attributes
            this.Load_Point_Attributes(pointData);
        }

        private void Load_Point_Attributes(vtkPointData pointData)
        {
            if (pointData == null)
            {
                Console.WriteLine("## Error: 'Get_Attribute_Values' PointData null");

                return;
            }

            int numArrays = pointData.GetNumberOfArrays();

            List<float> scalar_dat;
            List<Vector3> vector_dat;
            
            vtkDataArray dataArray;

            string arrayName;

            int arraysize = 0;

            float actvalue = 0;
            
            // runs over all attributes
            for (int i = 0; i < numArrays; i++)
            {
                scalar_dat = new List<float>();
                vector_dat = new List<Vector3>();

                dataArray = pointData.GetArray(i);
                
                arrayName = pointData.GetArrayName(i);
                

                arraysize = (int)dataArray.GetNumberOfTuples();                

                if (dataArray.GetNumberOfComponents() == 1)
                {
                    arrayNames.Add(arrayName);
                    if (this.scalar_dataArray.ContainsKey(arrayName))
                    {
                        this.scalar_dataArray[arrayName] = dataArray;
                    }
                    else
                    {
                        this.scalar_dataArray.Add(arrayName, dataArray);
                    }
                    for (int j = 0; j < arraysize; j++)
                    {
                        actvalue = (float)dataArray.GetTuple1(j);

                        scalar_dat.Add(actvalue);
                    }
                    
                    if (this.scalar_data.ContainsKey(arrayName))
                    {
                        this.scalar_data[arrayName] = scalar_dat;
                    }
                    else
                    {
                        this.scalar_data.Add(arrayName, scalar_dat);
                    }
                }
                else if (dataArray.GetNumberOfComponents() == 3)
                {
                   
                    
                    for (int j = 0; j < arraysize; j++)
                    {
                        double[] vec = dataArray.GetTuple3(j);

                        vector_dat.Add(new Vector3((float)vec[0], (float)vec[1], (float)vec[2]));
                    }
                    if (this.vector_data.ContainsKey(arrayName))
                    {
                        this.vector_data[arrayName] = vector_dat;
                    }
                    else
                    {
                        this.vector_data.Add(arrayName, vector_dat);                     

                    }
                }                

            }
        }
        private void WriteScalarDifferences(List<Vector4> vxDiff, string textFile)
        {
            string lineindex;
            using (StreamWriter sw = File.CreateText(textFile))
            {
                for (int i = 0; i < vxDiff.Count; i++)
                {
                    lineindex = vxDiff[i].ToString();

                    sw.WriteLine(lineindex);
                }

                sw.Flush();

                sw.Close();
            }
        }
        private void WriteTriangles(List<Vector3> triangleList, string textFile)
        {
            string lineindex;
            using (StreamWriter sw = File.CreateText(textFile))
            {
                for (int i = 0; i < triangleList.Count; i++)
                {
                    lineindex = triangleList[i].ToString();

                    sw.WriteLine(lineindex);
                }

                sw.Flush();

                sw.Close();
            }
        }
    }
}
