using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kitware.VTK;
using OpenTK;

namespace Project
{
    class Write
    {


        public void WriteVTM(string filePath, List<Vector3d> point_data, List<Vector4> tetrapoints, Dictionary<String, vtkDataArray> scalar_dataArray, List<String> arrayNames, int numberOfPoints, int numberOfTetraPoints)
        {

            vtkPoints points = vtkPoints.New();

            for (int i = 0; i < numberOfPoints; i++)
            {
                points.InsertNextPoint(point_data[i].X, point_data[i].Y, point_data[i].Z);

            }
            vtkMultiBlockDataSet multiBlockDataSet = vtkMultiBlockDataSet.New();
            
            double countBlocks = Math.Ceiling((double)((1.0*numberOfTetraPoints) / 1000000));
           
            multiBlockDataSet.SetNumberOfBlocks((uint)1);
            for (int j = 0; j < 1; j++)
            {                
                vtkUnstructuredGrid unstructuredGrid = vtkUnstructuredGrid.New();

                vtkCellArray cellArrayOne = vtkCellArray.New();
                int countTetras = 0;
                int startTetras = j * 1000000;
                if (numberOfTetraPoints < (j * 1000000) + 1000000)
                {
                    countTetras = numberOfTetraPoints;
                }
                else
                    countTetras = (j * 1000000) + 1000000;
                for (int i = startTetras; i < tetrapoints.Count(); i++)
                {
                    vtkTetra tetra = vtkTetra.New();

                    tetra.GetPointIds().SetId(0, (long)tetrapoints[i][0]);
                    tetra.GetPointIds().SetId(1, (long)tetrapoints[i][1]);
                    tetra.GetPointIds().SetId(2, (long)tetrapoints[i][2]);
                    tetra.GetPointIds().SetId(3, (long)tetrapoints[i][3]);

                    cellArrayOne.InsertNextCell(tetra);
                }
                //for (int i = 0; i < tetrapoints2.Count(); i++)
                //{
                //    vtkTetra tetra = vtkTetra.New();

                //    tetra.GetPointIds().SetId(0, (long)tetrapoints2[i][0]);
                //    tetra.GetPointIds().SetId(1, (long)tetrapoints2[i][1]);
                //    tetra.GetPointIds().SetId(2, (long)tetrapoints2[i][2]);
                //    tetra.GetPointIds().SetId(3, (long)tetrapoints2[i][3]);

                //    cellArrayOne.InsertNextCell(tetra);
                //}

                unstructuredGrid.SetPoints(points);

                const int VTK_TETRA = 10;
                unstructuredGrid.SetCells(VTK_TETRA, cellArrayOne);
                            

                int numberOfScalarData = scalar_dataArray.Count();
                for (int i = 0; i < numberOfScalarData; i++)
                {
                    scalar_dataArray.TryGetValue(arrayNames[i], out vtkDataArray scalars);
                    unstructuredGrid.GetPointData().AddArray(scalars);
                }
                multiBlockDataSet.SetBlock((uint)j, unstructuredGrid);
                

            }
            
            // add file ending if it is not existent
            string suffix = (".vtm");

            if (!(filePath.EndsWith(suffix)))
            {
                filePath += suffix;
            }

            // Write file
            vtkXMLMultiBlockDataWriter writer = vtkXMLMultiBlockDataWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(multiBlockDataSet);
            writer.Write();

            // Read and display file for verification that it was written correctly
            vtkXMLMultiBlockDataReader reader = vtkXMLMultiBlockDataReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                //MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("VTU file was writen and is saved at {0}", filePath);
        }

        public void WriteVTUTetra(string filePath, List<Vector3d> point_data, List<Vector4> tetrapoints, Dictionary<String, vtkDataArray> scalar_dataArray, List<String> arrayNames, int numberOfPoints, int numberOfTetraPoints)
        {

            vtkPoints points = vtkPoints.New();

            for (int i = 0; i < numberOfPoints; i++)
            {
                points.InsertNextPoint(point_data[i].X, point_data[i].Y, point_data[i].Z);

            }
           


            vtkUnstructuredGrid unstructuredGrid = vtkUnstructuredGrid.New();
            
            
               vtkCellArray cellArrayOne = vtkCellArray.New();

                for (int i = 0; i < numberOfTetraPoints; i++)
                {
                    vtkTetra tetra = vtkTetra.New();

                    tetra.GetPointIds().SetId(0, (long)tetrapoints[i][0]);
                    tetra.GetPointIds().SetId(1, (long)tetrapoints[i][1]);
                    tetra.GetPointIds().SetId(2, (long)tetrapoints[i][2]);
                    tetra.GetPointIds().SetId(3, (long)tetrapoints[i][3]);

                    cellArrayOne.InsertNextCell(tetra);
               
            }              


                unstructuredGrid.SetPoints(points);

                const int VTK_TETRA = 10;
                unstructuredGrid.SetCells(VTK_TETRA, cellArrayOne);




            int numberOfScalarData = scalar_dataArray.Count();
            for (int i = 0; i < numberOfScalarData; i++)
            {
                scalar_dataArray.TryGetValue(arrayNames[i], out vtkDataArray scalars);
                unstructuredGrid.GetPointData().AddArray(scalars);
            }

            // add file ending if it is not existent
            string suffix = (".vtu");

            if (!(filePath.EndsWith(suffix)))
            {
                filePath += suffix;
            }

            // Write file
            vtkXMLUnstructuredGridWriter writer = vtkXMLUnstructuredGridWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(unstructuredGrid);
            writer.Write();

            // Read and display file for verification that it was written correctly
            vtkXMLUnstructuredGridReader reader = vtkXMLUnstructuredGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                //MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("VTU file was writen and is saved at {0}", filePath);
        }


        public void WriteVTUTriangle(string filePath, List<Vector3d> point_data, List<Vector3> trianglepoints, Dictionary<String, vtkDataArray> scalar_dataArray, List<String> arrayNames, int numberOfPoints, int numberOfTetraPoints)
        {

            vtkPoints points = vtkPoints.New();

            for (int i = 0; i < numberOfPoints; i++)
            {
                points.InsertNextPoint(point_data[i].X, point_data[i].Y, point_data[i].Z);

            }

            vtkUnstructuredGrid unstructuredGrid = vtkUnstructuredGrid.New();


            vtkCellArray cellArrayOne = vtkCellArray.New();

            for (int i = 0; i < numberOfTetraPoints; i++)
            {

                vtkTriangle triangle = vtkTriangle.New();

                triangle.GetPointIds().SetId(0, (long)trianglepoints[i][0]);
                triangle.GetPointIds().SetId(1, (long)trianglepoints[i][1]);
                triangle.GetPointIds().SetId(2, (long)trianglepoints[i][2]);
               

                cellArrayOne.InsertNextCell(triangle);

            }


            unstructuredGrid.SetPoints(points);

            const int VTK_TRIANGLE = 5;
            unstructuredGrid.SetCells(VTK_TRIANGLE, cellArrayOne);




            int numberOfScalarData = scalar_dataArray.Count();
            for (int i = 0; i < numberOfScalarData; i++)
            {
                scalar_dataArray.TryGetValue(arrayNames[i], out vtkDataArray scalars);
                unstructuredGrid.GetPointData().AddArray(scalars);
            }

            // add file ending if it is not existent
            string suffix = (".vtu");

            if (!(filePath.EndsWith(suffix)))
            {
                filePath += suffix;
            }

            // Write file
            vtkXMLUnstructuredGridWriter writer = vtkXMLUnstructuredGridWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(unstructuredGrid);
            writer.Write();

            // Read and display file for verification that it was written correctly
            vtkXMLUnstructuredGridReader reader = vtkXMLUnstructuredGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                //MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("VTU file was writen and is saved at {0}", filePath);
        }


        public void WriteSimpleVTUExample()
        {
            string filePath = "C:\\DatenE\\02Studium\\02WiSe1718\\06IndividualProjekt\\03Daten\\testData\\simpleTest4Points.vtu";
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0, 0, 0);
            points.InsertNextPoint(1, 0, 0);
            points.InsertNextPoint(1, 1, 0);
            points.InsertNextPoint(0, 1, 1);

            points.InsertNextPoint(2, 0, 0);
            //points.InsertNextPoint(2, 2, 0);


            vtkTetra tetra = vtkTetra.New();

            tetra.GetPointIds().SetId(0, 0);
            tetra.GetPointIds().SetId(1, 1);
            tetra.GetPointIds().SetId(2, 2);
            tetra.GetPointIds().SetId(3, 3);

            vtkCellArray cellArray = vtkCellArray.New();
            cellArray.InsertNextCell(tetra);



            tetra.GetPointIds().SetId(0, 1);
            tetra.GetPointIds().SetId(1, 0);
            tetra.GetPointIds().SetId(2, 3);
            tetra.GetPointIds().SetId(3, 2);

            cellArray.InsertNextCell(tetra);
            tetra.GetPointIds().SetId(0, 0);
            tetra.GetPointIds().SetId(1, 2);
            tetra.GetPointIds().SetId(2, 3);
            tetra.GetPointIds().SetId(3, 1);

            cellArray.InsertNextCell(tetra);
            tetra.GetPointIds().SetId(0, 3);
            tetra.GetPointIds().SetId(1, 1);
            tetra.GetPointIds().SetId(2, 0);
            tetra.GetPointIds().SetId(3, 2);

            cellArray.InsertNextCell(tetra);

            tetra.GetPointIds().SetId(0, 3);
            tetra.GetPointIds().SetId(1, 0);
            tetra.GetPointIds().SetId(2, 2);
            tetra.GetPointIds().SetId(3, 4);

            cellArray.InsertNextCell(tetra);

            //tetra.GetPointIds().SetId(0, 4);
            //tetra.GetPointIds().SetId(1, 3);
            //tetra.GetPointIds().SetId(2, 0);
            //tetra.GetPointIds().SetId(3, 2);

            //cellArray.InsertNextCell(tetra);

            vtkUnstructuredGrid unstructuredGrid = vtkUnstructuredGrid.New();
            unstructuredGrid.SetPoints(points);
            const int VTK_TETRA = 10;
            unstructuredGrid.SetCells(VTK_TETRA, cellArray);


            // vX
            vtkDoubleArray scalarsX = new vtkDoubleArray();
            scalarsX.SetNumberOfValues(5);
            scalarsX.SetValue(0, 4);
            scalarsX.SetValue(1, 1);
            scalarsX.SetValue(2, 2);
            scalarsX.SetValue(3, 3);
            scalarsX.SetValue(4, 1);
            //scalarsX.SetValue(5, 2);
            scalarsX.SetName("Vx");
            unstructuredGrid.GetPointData().AddArray(scalarsX);
            // vY
            vtkDoubleArray scalarsY = new vtkDoubleArray();
            scalarsY.SetNumberOfValues(5);
            scalarsY.SetValue(0, 1);
            scalarsY.SetValue(1, 2);
            scalarsY.SetValue(2, 3);
            scalarsY.SetValue(3, 4);
            scalarsY.SetValue(4, 1);
            //scalarsY.SetValue(5, 2);
            scalarsY.SetName("Vy");
            unstructuredGrid.GetPointData().AddArray(scalarsY);
            // vZ
            vtkDoubleArray scalarsZ = new vtkDoubleArray();
            scalarsZ.SetNumberOfValues(5);
            scalarsZ.SetValue(0, 3);
            scalarsZ.SetValue(1,1);
            scalarsZ.SetValue(2, 4);
            scalarsZ.SetValue(3, 2);
            scalarsZ.SetValue(4, 1);
            //scalarsZ.SetValue(5, 2);
            scalarsZ.SetName("Vz");
            unstructuredGrid.GetPointData().AddArray(scalarsZ);


            // Write file
            vtkXMLUnstructuredGridWriter writer = vtkXMLUnstructuredGridWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(unstructuredGrid);
            writer.Write();
        }

        public void WriteStructuredGrid(string filePath, List<Vector3d> point_data, Dictionary<String, vtkDataArray> scalar_dataArray, List<String> arrayNames, int numberOfPoints, int[] dimensions)
        {
            vtkPoints points = vtkPoints.New();

            for (int i = 0; i < numberOfPoints; i++)
            {
                points.InsertNextPoint(point_data[i].X, point_data[i].Y, point_data[i].Z);

            }

            vtkStructuredGrid structuredGrid = vtkStructuredGrid.New();
            structuredGrid.SetDimensions(dimensions[0], dimensions[1], dimensions[2]);
            structuredGrid.SetPoints(points);

            int numberOfScalarData = scalar_dataArray.Count();
            for (int i = 0; i < numberOfScalarData; i++)
            {
                scalar_dataArray.TryGetValue(arrayNames[i], out vtkDataArray scalars);
                structuredGrid.GetPointData().AddArray(scalars);
            }

            // add file ending if it is not existent
            string suffix = (".vts");

            if (!(filePath.EndsWith(suffix)))
            {
                filePath += suffix;
            }

            // Write file
            vtkXMLStructuredGridWriter writer = vtkXMLStructuredGridWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(structuredGrid);
            writer.Write();

            // Read and display file for verification that it was written correctly
            vtkXMLStructuredGridReader reader = vtkXMLStructuredGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                //MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("VTU file was writen and is saved at {0}", filePath);
        }

        public void WritSimpleVTM()
        {
            string filePath = "..\\..\\..\\..\\..\\03Daten\\testData\\simpleTest8Points.vtm";
            vtkMultiBlockDataSet multiBlockDataSet = vtkMultiBlockDataSet.New();
            multiBlockDataSet.SetNumberOfBlocks(2);

            vtkUnstructuredGrid unstructuredGrid1 = vtkUnstructuredGrid.New();

            vtkPoints points1 = vtkPoints.New();
            points1.InsertNextPoint(0, 0, 0);
            points1.InsertNextPoint(1, 0, 0);
            points1.InsertNextPoint(1, 1, 0);
            points1.InsertNextPoint(0, 1, 1);

            vtkPoints points2 = vtkPoints.New();
            points1.InsertNextPoint(2, 0, 0);
            points1.InsertNextPoint(2, 2, 0);
            points1.InsertNextPoint(2, 2, 3);
            points1.InsertNextPoint(0, 2, 3);

            vtkTetra tetra = vtkTetra.New();

            tetra.GetPointIds().SetId(0, 0);
            tetra.GetPointIds().SetId(1, 1);
            tetra.GetPointIds().SetId(2, 2);
            tetra.GetPointIds().SetId(3, 3);

            vtkCellArray cellArray1 = vtkCellArray.New();
            cellArray1.InsertNextCell(tetra);

            unstructuredGrid1.SetPoints(points1);
            const int VTK_TETRA = 10;
            unstructuredGrid1.SetCells(VTK_TETRA, cellArray1);

            vtkCellArray cellArray2 = vtkCellArray.New();
            tetra = vtkTetra.New();
            tetra.GetPointIds().SetId(0, 4);
            tetra.GetPointIds().SetId(1, 5);
            tetra.GetPointIds().SetId(2, 6);
            tetra.GetPointIds().SetId(3, 7);
            cellArray2.InsertNextCell(tetra);
            tetra = vtkTetra.New();
            tetra.GetPointIds().SetId(0, 7);
            tetra.GetPointIds().SetId(1, 5);
            tetra.GetPointIds().SetId(2, 2);
            tetra.GetPointIds().SetId(3, 4);

            cellArray2.InsertNextCell(tetra);
            

            vtkUnstructuredGrid unstructuredGrid = vtkUnstructuredGrid.New();
            unstructuredGrid.SetPoints(points1);
            unstructuredGrid.SetCells(VTK_TETRA, cellArray2);

            multiBlockDataSet.SetBlock(0, unstructuredGrid1);
            multiBlockDataSet.SetBlock(1, unstructuredGrid);

            // Write file
            vtkXMLMultiBlockDataWriter writer = vtkXMLMultiBlockDataWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(multiBlockDataSet);
            writer.Write();
        }

        public void WriteSimpleStructuredGrid(string filePath)
        {
            vtkPoints points = vtkPoints.New();

            double x, y, z;

            x = 0.0;
            y = 0.0;
            z = 0.0;

            for (uint k = 0; k < 2; k++)
            {
                z += 2.0;
                for (uint  j = 0; j < 3; j++)
                {
                    y += 1.0;
                    for (uint i = 0; i < 2; i++)
                    {
                        x += .5;
                        points.InsertNextPoint(x, y, z);
                    }
                }
            }

            vtkDataArray dataArrayVx;
            vtkDataArray dataArrayVy;
            vtkDataArray dataArrayVz;

            vtkDoubleArray Vx = new vtkDoubleArray();
            vtkDoubleArray Vy = new vtkDoubleArray();
            vtkDoubleArray Vz = new vtkDoubleArray();
            for (int i = 0; i < 12; i++)
            {
                Vx.InsertNextValue(-1+i);
                Vz.InsertNextValue(2-i);
                Vy.InsertNextValue(-4+i);
            }
            dataArrayVx = Vx;
            dataArrayVx.SetName("Vx");
            dataArrayVy = Vy;
            dataArrayVy.SetName("Vy");
            dataArrayVz = Vz;
            dataArrayVz.SetName("Vz");
            

            vtkStructuredGrid structuredGrid = vtkStructuredGrid.New();
            structuredGrid.SetDimensions(2,3, 2);
            structuredGrid.SetPoints(points);

            structuredGrid.GetPointData().AddArray(dataArrayVx);
            structuredGrid.GetPointData().AddArray(dataArrayVy);
            structuredGrid.GetPointData().AddArray(dataArrayVz);

            // add file ending if it is not existent
            string suffix = (".vts");

            if (!(filePath.EndsWith(suffix)))
            {
                filePath += suffix;
            }
            // Write file
            vtkXMLStructuredGridWriter writer = vtkXMLStructuredGridWriter.New();
            writer.SetFileName(filePath);
            writer.SetInput(structuredGrid);
            writer.Write();

            // Read and display file for verification that it was written correctly
            vtkXMLStructuredGridReader reader = vtkXMLStructuredGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                //MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("VTU file was writen and is saved at {0}", filePath);
        }

        public void WriteVTP(string filePath, List<Vector3d> point_data, List<Vector3> cellPointsList, Dictionary<String, vtkDataArray> scalar_dataArray, List<String> arrayNames)
        {
            vtkPoints points = vtkPoints.New();

            for (int i = 0; i < point_data.Count(); i++)
            {
                points.InsertNextPoint(point_data[i].X, point_data[i].Y, point_data[i].Z);
            }

            vtkPolyData polyData = vtkPolyData.New();
            polyData.SetPoints(points);

            vtkCellArray cellArrayOne = vtkCellArray.New();

            for (int i = 0; i < cellPointsList.Count(); i++ )
            {
                vtkTetra tetra = vtkTetra.New();

                tetra.GetPointIds().SetId(0, (long)cellPointsList[i][0]);
                tetra.GetPointIds().SetId(1, (long)cellPointsList[i][1]);
                tetra.GetPointIds().SetId(2, (long)cellPointsList[i][2]);
                tetra.GetPointIds().SetId(3, (long)cellPointsList[i][2]);
                cellArrayOne.InsertNextCell(tetra);

            }

            polyData.SetPolys(cellArrayOne);

            int numberOfScalarData = scalar_dataArray.Count();
            for (int i = 0; i < numberOfScalarData; i++)
            {
                scalar_dataArray.TryGetValue(arrayNames[i], out vtkDataArray scalars);
                polyData.GetPointData().AddArray(scalars);
            }

            vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();

            // add file ending if it is not existent
            string suffix = (".vtp");

            if (!(filePath.EndsWith(suffix)))
            {
                filePath += suffix;
            }

            writer.SetFileName(filePath);
            writer.SetInput(polyData);
            writer.Write();

            // Read and display file for verification that it was written correctly
            vtkXMLStructuredGridReader reader = vtkXMLStructuredGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                //MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("VTP file was writen and is saved at {0}", filePath);
        }
       
    }
}
