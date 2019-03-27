using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTK;

using MathWorks.MATLAB.NET.Arrays;
using ICPCalculation;
using System.IO;

using Kitware.VTK;
using Accord.Math.Decompositions;
using System.Windows.Media.Media3D;
//using EigenvalueDecomposition;
using SolveLGS;
namespace Project
{
    class Algorithms
    {

        public List<Vector4d> transMatrix = new List<Vector4d>();

        public Dictionary<string, List<double>> scalarData_Array = new Dictionary<string, List<double>>();
        public Dictionary<String, vtkDataArray> scalar_dataArray = new Dictionary<string, vtkDataArray>();
        public List<string> scalarNames = new List<string>();

        public List<String> arrayNames = new List<String>();
        public int counterNN;

        public List<Vector3d> ICPCalculation(List<Vector3d> pointsToTransform, List<Vector3d> pointsOrig)
        {
            int j = 0;
            int m = 0;
            double[,] map_points = new double[3, pointsToTransform.Count];
            for (int i = 0; i < map_points.Length/ 3; i++, j++)
            {
                map_points.SetValue(pointsToTransform[i].X, 0, j);
                map_points.SetValue(pointsToTransform[i].Y, 1, j);
                map_points.SetValue(pointsToTransform[i].Z, 2, j);
            }
            double[,] map_pointsToTransform = ThinPointCloud(pointsToTransform, 0,50);
            m += 10;
            double[,] map_pointsOrig = new double[3, pointsOrig.Count()];
            for (int i = 0; i < pointsOrig.Count; i++)
            {
                map_pointsOrig.SetValue(pointsOrig[i].X, 0, i);
                map_pointsOrig.SetValue(pointsOrig[i].Y, 1, i);
                map_pointsOrig.SetValue(pointsOrig[i].Z, 2, i);
            }
            //IterativeClosestPointClass icp = new IterativeClosestPointClass();
            ICP icpTest = new ICP();
           // ICP icpTest = new ICP();
            MWCellArray outMapedPointsNew = null;
            MWCellArray outMapedPointsOld = null;

            Console.WriteLine("Start of ICP algorithm");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            outMapedPointsNew = (MWCellArray)icpTest.getIterativeClosestPoints((MWNumericArray)map_pointsOrig, (MWNumericArray)map_pointsToTransform, (MWNumericArray)map_points, (MWNumericArray)20);

            for (int i = 0; i < 5; i++)
            {
                m += 5;
                MWNumericArray newoutmappedPoints = (MWNumericArray)outMapedPointsNew[1];
                List<Vector3d> newPoints = new List<Vector3d>();

                double newx, newy, newz;
                Vector3d newact_point;
                for (int k = 1; k < newoutmappedPoints.Dimensions[1]; k++)
                {

                    newx = (double)newoutmappedPoints[1, k];
                    newy = (double)newoutmappedPoints[2, k];
                    newz = (double)newoutmappedPoints[3, k];

                    newact_point = new Vector3d(newx, newy, newz);

                    newPoints.Add(newact_point);
                }
                map_pointsToTransform = ThinPointCloud(newPoints, m,50);
                outMapedPointsOld = outMapedPointsNew;

                // Call function getIterativeClosestPoints which returns a new Array of map_points with rotation and translation
                //outMapedPoints= (MWCellArray)icp.getIterativeClosestPoints((MWNumericArray)map_pointsOrig, (MWNumericArray)map_points, (MWNumericArray)map_pointsToTransform,(MWNumericArray)15);
                outMapedPointsNew = (MWCellArray)icpTest.getIterativeClosestPoints((MWNumericArray)map_pointsOrig, (MWNumericArray)map_pointsToTransform, outMapedPointsOld[1], (MWNumericArray)30);

            }

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine("End of ICP algorithm, it took : {0} minuts and {1} seconds!", ts.Minutes,ts.Seconds);    
            MWNumericArray mappedPoints = (MWNumericArray)outMapedPointsNew[1];
            MWNumericArray rotation_vec = (MWNumericArray)outMapedPointsNew[2];
            MWNumericArray translation_vec = (MWNumericArray)outMapedPointsNew[3];
            
            Vector3d act_point;

            List<Vector3d> points = new List<Vector3d>();

            double x, y, z;

            for (int i = 1; i <= mappedPoints.Dimensions[1]; i++)
            {

                x = (double)mappedPoints[1, i];
                y = (double)mappedPoints[2, i];
                z = (double)mappedPoints[3, i];

                act_point = new Vector3d(x, y, z);

                points.Add(act_point);
            }
            
            List<Vector4d> matrix = new List<Vector4d>();
            Vector4d column;
            for (int i = 1; i < 4; i++)
            {

                x = (double)mappedPoints[1, i];
                y = (double)mappedPoints[2, i];
                z = (double)mappedPoints[3, i];

                column = new Vector4d((double)rotation_vec[i,1], (double)rotation_vec[i, 2], (double)rotation_vec[i, 3], (double)translation_vec[i, 1]);

                matrix.Add(column);
            }
            column = new Vector4d(0, 0, 0, 1);
            matrix.Add(column);
            //WriteTransformationMatrix(matrix, "..\\..\\..\\..\\..\\03Daten\\registratedData\\MeshTomo_to_SimulatedTransformation.txt");
            transMatrix = matrix;
            //GetNearestPoints(points, pointsOrig);
            return points;
        }


        public List<int> GetNearestPoints(List<Vector3d> points, List<Vector3d> pointsOrig, Dictionary<String, List<float>> scalarDataRegistratedPoints, Dictionary<String, List<float>> scalarDataOriginalPoints)
        {
            //Dictionary<string, List<float>>.KeyCollection KeyCollRegistrated = scalarDataRegistratedPoints.Keys;
            //int nKeysRegistrated = KeyCollRegistrated.Count();
            //Dictionary<string, List<float>>.KeyCollection KeyCollOriginal = scalarDataOriginalPoints.Keys;
            //int nKeysOriginal = KeyCollOriginal.Count();

            //List<float> scalarValuesX = new List<float>();
            //scalarDataOriginalPoints.TryGetValue(KeyCollOriginal.ElementAt<string>(0), out scalarValuesX);
            //List<float> scalarValuesY = new List<float>();
            //scalarDataOriginalPoints.TryGetValue(KeyCollOriginal.ElementAt<string>(1), out scalarValuesY);
            //List<float> scalarValuesZ = new List<float>();
            //scalarDataOriginalPoints.TryGetValue(KeyCollOriginal.ElementAt<string>(2), out scalarValuesZ);

            //// create vectorfield
            //List<Vector3d> vectorFielRegistratedPoints = new List<Vector3d>();
            //for (int i = 0; i < scalarValuesX.Count(); i++)
            //{
            //    Vector3d vec = new Vector3d(scalarValuesX[i], scalarValuesY[i], scalarValuesZ[i]);

            //    vectorFielRegistratedPoints.Add(vec);
            //}


            //scalarDataRegistratedPoints.TryGetValue("x_velocity", out scalarValuesX);

            //scalarDataRegistratedPoints.TryGetValue("y_velocity", out scalarValuesY);

            //scalarDataRegistratedPoints.TryGetValue("z_velocity", out scalarValuesZ);

            //// create vectorfield
            //List<Vector3d> vectorFielOriginalPoints = new List<Vector3d>();
            //for (int i = 0; i < scalarValuesX.Count(); i++)
            //{
            //    Vector3d vec = new Vector3d(scalarValuesX[i], scalarValuesY[i], scalarValuesZ[i]);

            //    vectorFielOriginalPoints.Add(vec);
            //}

            NearestNeighbor.NearestNeighbor interpolation = new NearestNeighbor.NearestNeighbor();

            double[,] mappedPointsArray = new double[points.Count(),3];
            for (int i = 0; i< points.Count(); i++)
            {
                //if (vectorFielRegistratedPoints[i] != new Vector3d(0, 0, 0))
                //{
                    mappedPointsArray.SetValue(points[i].X, i, 0);
                    mappedPointsArray.SetValue(points[i].Y, i, 1);
                    mappedPointsArray.SetValue(points[i].Z, i, 2);                    
                //}
            }
            double[,] _pointsOrig = new double[pointsOrig.Count(), 3];
            for (int i = 0; i < _pointsOrig.GetLength(0); i++)
            {
                //if (vectorFielRegistratedPoints[i] != new Vector3d(0, 0, 0))
                //{
                    _pointsOrig.SetValue(pointsOrig[i].X, i, 0);
                    _pointsOrig.SetValue(pointsOrig[i].Y, i, 1);
                    _pointsOrig.SetValue(pointsOrig[i].Z, i, 2);
                //}
            }
            MWCellArray outInterpolatedArray = null;
            Console.WriteLine("Start of nearest point interpolation algorithm");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Interpolate between outNewPointsMono and map_pointsStereo            
            // finds the nearest neighbor in _pointsOrig for each point in mappedPointsArray. outInterpolatedArray is a column vector with my rows. 
            // Each row in outInterpolatedArray contains the index of nearest neighbor in _pointsOrig for the corresponding row in mappedPointsArray.
            outInterpolatedArray = (MWCellArray)interpolation.getnearestPosition((MWNumericArray)_pointsOrig, (MWNumericArray)mappedPointsArray);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine("End of NNInterpolation algorithm, it took : {0} minuts and {1} seconds!", ts.Minutes, ts.Seconds);

            MWNumericArray outInterpolatedNumericArray = (MWNumericArray)outInterpolatedArray[1];

            List<int> l_interpolatedPoints = new List<int>();
            List<Vector3d> interpolatedMappedPoints = new List<Vector3d>();
            
            for (int i = 1; i < outInterpolatedNumericArray.Dimensions[0]+1; i++)
            {
                int interpolatedPoint = (int)outInterpolatedNumericArray[i, 1];
                l_interpolatedPoints.Add(interpolatedPoint);
            }
            return l_interpolatedPoints;
        }

        public void CalculateDifferences(List<Vector3d> registratedPoints, List<Vector3d> originalPoints, Dictionary<String, List<float>> scalarDataRegistratedPoints, Dictionary<String, List<float>> scalarDataOriginalPoints, List<int> interpolatedPointArray)
        {
            Dictionary<string, List<float>>.KeyCollection KeyCollRegistrated = scalarDataRegistratedPoints.Keys;
            int nKeysRegistrated = KeyCollRegistrated.Count();
            Dictionary<string, List<float>>.KeyCollection KeyCollOriginal = scalarDataOriginalPoints.Keys;
            int nKeysOriginal = KeyCollOriginal.Count();

            List<float> scalarValuesX = new List<float>();
            scalarDataOriginalPoints.TryGetValue(KeyCollOriginal.ElementAt<string>(0), out scalarValuesX);
            List<float> scalarValuesY = new List<float>();
            scalarDataOriginalPoints.TryGetValue(KeyCollOriginal.ElementAt<string>(1), out scalarValuesY);
            List<float> scalarValuesZ = new List<float>();
            scalarDataOriginalPoints.TryGetValue(KeyCollOriginal.ElementAt<string>(2), out scalarValuesZ);

            double maxVec1 = Double.MinValue;
            double maxVec2 = Double.MinValue;
            // create vectorfield
            List<Vector3d> vectorFielRegistratedPoints = new List<Vector3d>();
            for (int i = 0; i < scalarValuesX.Count(); i++)
            {
                Vector3d vec = new Vector3d(scalarValuesX[i], scalarValuesY[i], scalarValuesZ[i]);

                vectorFielRegistratedPoints.Add(vec);
                maxVec1 = Math.Max(vec.Length, maxVec1);
            }


            scalarDataRegistratedPoints.TryGetValue(KeyCollRegistrated.ElementAt<string>(0), out scalarValuesX);

            scalarDataRegistratedPoints.TryGetValue(KeyCollRegistrated.ElementAt<string>(1), out scalarValuesY);

            scalarDataRegistratedPoints.TryGetValue(KeyCollRegistrated.ElementAt<string>(2), out scalarValuesZ);

            // create vectorfield
            List<Vector3d> vectorFielOriginalPoints = new List<Vector3d>();
            for (int i = 0; i < scalarValuesX.Count(); i++)
            {
                Vector3d vec = new Vector3d(scalarValuesX[i], scalarValuesY[i], scalarValuesZ[i]);

                vectorFielOriginalPoints.Add(vec);
                maxVec2 = Math.Max(vec.Length, maxVec2);
            }


            List<double> oneDiff = new List<double>();
            List<double> anglesList = new List<double>();
            
            vtkDoubleArray scalars = new vtkDoubleArray();
            vtkDoubleArray anglesArray = new vtkDoubleArray();
            vtkDoubleArray VxDifferences = new vtkDoubleArray();
            scalars.SetNumberOfValues(registratedPoints.Count());
            anglesArray.SetNumberOfValues(registratedPoints.Count());
            VxDifferences.SetNumberOfValues(registratedPoints.Count());
            int counter = 0;
            for (int j = 0; j < registratedPoints.Count(); j++)
            {
                double diff = 0;
                double angle = 0.0;
                double VxDiff = 0.0;
                if (vectorFielRegistratedPoints.Count() > j)                
                {
                    Vector3d test2 = vectorFielRegistratedPoints[interpolatedPointArray[j]];
                    //if (test2 != new Vector3d(0, 0, 0))
                    //{                        
                       
                        diff = Math.Abs(vectorFielRegistratedPoints[interpolatedPointArray[j]].Length - vectorFielOriginalPoints[j].Length);
                        Vector3D vec1 = new Vector3D(vectorFielRegistratedPoints[interpolatedPointArray[j]].X, vectorFielRegistratedPoints[interpolatedPointArray[j]].Y, vectorFielRegistratedPoints[interpolatedPointArray[j]].Z);
                        Vector3D vec2 = new Vector3D(vectorFielOriginalPoints[j].X, vectorFielOriginalPoints[j].Y, vectorFielOriginalPoints[j].Z);
                        if(vec1 != new Vector3D(0, 0, 0) && vec2 != new Vector3D(0, 0, 0))
                            angle = Vector3D.AngleBetween(vec1, vec2);
                       
                        Vector3D vec1T = new Vector3D(10,10,10);
                        Vector3D vec2T = new Vector3D(-10, -10, -10);
                       double angleTest = Vector3D.AngleBetween(vec1T, vec2T);
                        VxDiff = Math.Abs(vectorFielRegistratedPoints[interpolatedPointArray[j]].X - vectorFielOriginalPoints[j].X);                    
                    //}
                    if(test2 == new Vector3d(0, 0, 0))
                    {
                        counter++;
                    }
                }

                oneDiff.Add(diff);

                scalars.SetValue(j,diff);
                anglesArray.SetValue(j, angle);
                VxDifferences.SetValue(j, VxDiff);
            }
            vtkDataArray dataArray;
            dataArray = scalars;
                
            dataArray.SetName("VelocityDifferences");
 
            this.scalarData_Array.Add("VelocityDifferences", oneDiff);
            this.scalarNames.Add("VelocityDifferences");


            if (this.scalar_dataArray.ContainsKey("VelocityDifferences"))
            {
                this.scalar_dataArray["VelocityDifferences"] = dataArray;
            }
            else
            {
                this.scalar_dataArray.Add("VelocityDifferences", dataArray);
                this.arrayNames.Add("VelocityDifferences");
            }

            dataArray = anglesArray;
            dataArray.SetName("Angles");

            this.scalarData_Array.Add("Angles", oneDiff);
            this.scalarNames.Add("Angles");


            if (this.scalar_dataArray.ContainsKey("Angles"))
            {
                this.scalar_dataArray["Angles"] = dataArray;
            }
            else
            {
                this.scalar_dataArray.Add("Angles", dataArray);
                this.arrayNames.Add("Angles");
            }

            dataArray = VxDifferences;
            dataArray.SetName("VxDifferences");

            this.scalarData_Array.Add("VxDifferences", oneDiff);
            this.scalarNames.Add("VxDifferences");


            if (this.scalar_dataArray.ContainsKey("VxDifferences"))
            {
                this.scalar_dataArray["VxDifferences"] = dataArray;
            }
            else
            {
                this.scalar_dataArray.Add("VxDifferences", dataArray);
                this.arrayNames.Add("VxDifferences");
            }

        }

        private double[,] ThinPointCloud(List<Vector3d> originalPoints, int startPos, int percentage)
        {
            double resultSize = originalPoints.Count - (originalPoints.Count * (percentage*1.0/100));
            double stepsize = originalPoints.Count / resultSize;
            int j = 0;
            double[,] thinnedPoints = new double[3,originalPoints.Count()/ (int)stepsize];
            for (int i = startPos; i < thinnedPoints.Length / 3; i++, j += (int)stepsize)
            {
                thinnedPoints.SetValue(originalPoints[j].X, 0, i);
                thinnedPoints.SetValue(originalPoints[j].Y, 1, i);
                thinnedPoints.SetValue(originalPoints[j].Z, 2, i);
            }
            return thinnedPoints;
        }

        public void CalculateLambdaCriterionRegularGrids(List<Vector3d> pointsToRegistrate, Dictionary<string, List<float>> scalarValues, List<string> scalarNames, int[] dimensions)
        {
            List<float> scalarValuesX = new List<float>();
            scalarValues.TryGetValue(scalarNames[0], out scalarValuesX);
            List<float> scalarValuesY = new List<float>();
            scalarValues.TryGetValue(scalarNames[1], out scalarValuesY);
            List<float> scalarValuesZ = new List<float>();
            scalarValues.TryGetValue(scalarNames[2], out scalarValuesZ);

            // create vectorfield
            List<Vector3d> vectorField = new List<Vector3d>();
            for (int i = 0; i < scalarValuesX.Count(); i++)
            {
                Vector3d vec = new Vector3d(scalarValuesX[i],scalarValuesY[i],scalarValuesZ[i]);               
               
                vectorField.Add(vec);
            }          
            
            vtkDoubleArray lambda2Values = new vtkDoubleArray();
           // lambda2Values.SetNumberOfValues(vectorField.Count());
            
            int sizeJacobimat = 3;
          
            // iterating over all points 
            for (int k = 0; k < vectorField.Count(); k++)
            {
                if (k == 356003)
                    Console.WriteLine("ID");
                double[,] jacobiMat = new double[sizeJacobimat, sizeJacobimat];
               
                for (int j = 0, i = 0; j < sizeJacobimat; j++, i++)
                {                  
                   
                    Vector3d vec = vectorField[k];

                    Vector3d gradVec = new Vector3d();
                    // first column of jacobi matrix
                    if (i == 0)
                    {
                        // forward difference
                        if (k == 0)
                        {
                            double hx = Math.Abs(pointsToRegistrate[k + 1].X - pointsToRegistrate[k].X);
                            gradVec = (vectorField[k + 1] - vec) / hx;
                        }
                        // backward difference
                        else if(k == vectorField.Count() - 1)
                        {
                            double hx = Math.Abs(pointsToRegistrate[k - 1].X - pointsToRegistrate[k].X);
                            gradVec = (vec - vectorField[k - 1]) / hx;
                        }
                        // central difference
                        else
                        {
                            double hx = Math.Abs(pointsToRegistrate[k + 1].X - pointsToRegistrate[k].X);
                            Vector3d f = vectorField[k + 1];
                            Vector3d b = vectorField[k - 1];
                            gradVec = (f - b) / (2 * hx);
                        }
                    }
                    // second column of jacobi matrix
                    else if (i == 1)
                    {
                        // forward difference
                        if (k - dimensions[0] < 0)
                        {
                            double hy = Math.Abs(pointsToRegistrate[k + dimensions[0]].Y - pointsToRegistrate[k].Y);
                            gradVec = (vectorField[k + dimensions[0]] - vec) / hy;
                        }
                        // backward difference
                        else if (k +dimensions[0] > vectorField.Count()-1)
                        {
                            double hy = Math.Abs(pointsToRegistrate[k - dimensions[0]].Y - pointsToRegistrate[k].Y);
                            gradVec = (vec - vectorField[k - dimensions[0]]) / hy;
                        }
                        // central difference
                        else
                        {
                            double hy = Math.Abs(pointsToRegistrate[k + dimensions[0]].Y - pointsToRegistrate[k].Y);
                            Vector3d f = vectorField[k + dimensions[0]];
                            Vector3d b = vectorField[k - dimensions[0]];
                            gradVec = (f - b) / (2 * hy);
                        }
                    }
                    // third column of jacobi matrix
                    else if (i == 2)
                    {
                        // forward difference
                        if (k - (dimensions[0] * dimensions[1]) < 0)
                        {
                            double hz = Math.Abs(pointsToRegistrate[k + (dimensions[0] * dimensions[1])].Z - pointsToRegistrate[k].Z);
                            gradVec = (vectorField[k + (dimensions[0] * dimensions[1])] - vec) / hz;
                        }
                        // backward difference
                        else if (k + (dimensions[0] * dimensions[1]) > vectorField.Count()-1)
                        {
                            double hz = Math.Abs(pointsToRegistrate[k - (dimensions[0] * dimensions[1])].Z - pointsToRegistrate[k].Z);
                            gradVec = (vec - vectorField[k - (dimensions[0] * dimensions[1])]) / hz;
                        }
                        // central difference
                        else
                        {
                            double hz = Math.Abs(pointsToRegistrate[k + (dimensions[0] * dimensions[1])].Z - pointsToRegistrate[k].Z);
                            Vector3d f = vectorField[k + (dimensions[0] * dimensions[1])];
                            Vector3d b = vectorField[k - (dimensions[0] * dimensions[1])];
                            gradVec = (f - b) / (2 * hz);
                        }
                    }
                    // fill jacobi matrix
                    jacobiMat[0, j] = gradVec[0];
                    jacobiMat[1, j] = gradVec[1];
                    if (sizeJacobimat == 3)
                        jacobiMat[2, j] = gradVec[2];
                }
                // only if diagonal elements are not equal zero
                if (jacobiMat[1, 1] == 0 && jacobiMat[0, 0] == 0 && jacobiMat[2, 2] == 0)
                             lambda2Values.InsertNextValue(0);
                else
                {
                    MatrixOperations classObj = new MatrixOperations();                

                    double[,] transposedJacobiMat = classObj.TransposeRowsAndColumns(jacobiMat);
                    // calculation of S und Omega
                    double[,] S = classObj.DivideArrayByValue(classObj.AddArrays(jacobiMat, transposedJacobiMat), 2);
                    double[,] squaredS = classObj.MultiplyArrays(S, S);

                    double[,] Omega = classObj.DivideArrayByValue(classObj.SubtractArrays(jacobiMat, transposedJacobiMat), 2);
                    double[,] squaredOmega = classObj.MultiplyArrays(Omega, Omega);

                    double[,] summation = classObj.AddArrays(squaredS, squaredOmega);

                    var eigenvalueDecomposition = new Accord.Math.Decompositions.EigenvalueDecomposition(summation, false, false, true);
                    var eigenVec = eigenvalueDecomposition.Eigenvectors;
                    var eigenValue = eigenvalueDecomposition.RealEigenvalues;
                    double lambda2 = eigenValue[1];                    
          
                   
                    lambda2Values.InsertNextValue(lambda2);
                }

            }
            vtkDataArray dataArray;
            dataArray = lambda2Values;
            dataArray.SetName("lambda2");
            Console.WriteLine("All lambda2 were calculated!");
            if ( this.scalar_dataArray.ContainsKey("lambda2"))
                this.scalar_dataArray["lambda2"] = lambda2Values;

            else
            {
                this.scalar_dataArray.Add("lambda2", dataArray);
                this.arrayNames.Add("lambda2");
            }  
        }


        public void CalculateLambda2CriterionIrregularGrids(List<Vector3d> pointsToRegistrate, Dictionary<string, List<float>> scalarValues, List<string> scalarNames, List<Vector4> tetraPoints)
        {
            List<float> scalarValuesX = new List<float>();
            List<float> scalarValuesY = new List<float>();
            List<float> scalarValuesZ = new List<float>();

            //if (scalarNames.Count() <= 4)
            //{
                scalarValues.TryGetValue(scalarNames[0], out scalarValuesX);
                scalarValues.TryGetValue(scalarNames[1], out scalarValuesY);
                scalarValues.TryGetValue(scalarNames[2], out scalarValuesZ);
            //}
            //else
            //{
            //    scalarValues.TryGetValue(scalarNames[1], out scalarValuesX);
            //    scalarValues.TryGetValue(scalarNames[2], out scalarValuesY);
            //    scalarValues.TryGetValue(scalarNames[3], out scalarValuesZ);
            //}

            vtkDoubleArray lambda2Values = new vtkDoubleArray();
            List<double> lambda2ValuesList = new List<double>();

            // create vectorfield
            List<Vector3d> vectorField = new List<Vector3d>();
            for (int i = 0; i < scalarValuesX.Count(); i++)
            {
                Vector3d vec = new Vector3d(scalarValuesX[i], scalarValuesY[i], scalarValuesZ[i]);

                vectorField.Add(vec);
            }

            int count = 0;
            double[,] lambda2ValuesArray = new double[pointsToRegistrate.Count(),2];
            for (int k = 0; k < tetraPoints.Count(); k++)
            {
                // first rows and than columns 
                double[,] A = new double[12, 12];
                double[,] rightSide = new double[12, 1];
                int i = 0, j = 0;
                int s = 0;

                if (k == 779834)
                    Console.WriteLine("Test");

                for (int g = 0; g < 12; g += 3, i++, s++)
                { 
                    //1-3 columns of 4x3 Matrix
                    for (int l = 0; l < 3; l++)
                    {
                        for (int m = 0; m < 3; m++)
                        {                            
                            if (l == m)
                                // Warum tetraPoints[k]
                                A[g + l, m] = pointsToRegistrate[(int)tetraPoints[k][i]].X;
                            else
                                A[g + l, m] = 0;
                        }
                    }
                    for (int l = 0; l < 3; l++)
                    {
                        for (int m = 3; m < 6; m++)
                        {
                            if (l == (m-3))
                                A[g + l, m] = pointsToRegistrate[(int)tetraPoints[k][i]].Y;
                            else
                                A[g + l, m] = 0;
                        }
                    }
                    for (int l = 0; l < 3; l++)
                    {
                        for (int m = 6; m < 9; m++)
                        {
                            if (l == (m - 6))
                                A[g + l, m] = pointsToRegistrate[(int)tetraPoints[k][i]].Z;
                            else
                                A[g + l, m] = 0;
                        }
                    }

                    for (int l = 0; l < 3; l++)
                    {
                        for (int m = 9; m < 12; m++)
                        {
                            if (l == (m - 9))
                                A[g+l, m] = 1;
                            else
                                A[g+l, m] = 0;
                        }
                    }                    
                  
                    rightSide[g, 0] = vectorField[(int)tetraPoints[k][s]].X;
                    rightSide[g + 1, 0] = vectorField[(int)tetraPoints[k][s]].Y;
                    rightSide[g + 2, 0] = vectorField[(int)tetraPoints[k][s]].Z;
                   
                }

                //for (int l = 0; l < 12; l++)
                //{
                //    for (int m = 0; m < 12; m++)
                //    {
                //        Console.Write(A[l, m] + " ");
                //    }
                //    Console.WriteLine("");
                //}



                double[,] Vold = Accord.Math.Matrix.Solve(A, rightSide, leastSquares: true);
                // Solve matrix with matlab
                //LGS lgsObj = new LGS();
                //MWNumericArray mwV = (MWNumericArray)lgsObj.solveLGS((MWNumericArray)A, (MWNumericArray)rightSide);
                //double[,] V = new double[mwV.Dimensions[0], 1];
                //for (i = 1; i <= mwV.Dimensions[1]; i++)
                //{
                //    V[i, 0] = (double)mwV[1, i];
                //}

                double[,] jacobiMat = new double[3, 3];
                int n = 0;
                for (i = 0; i < 3; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        // first increas rows and than columns
                        jacobiMat[j, i] = Vold[n, 0];
                        //jacobiMat[i, j] = Vold[n, 0];
                        n++;
                    }
                }

                if (jacobiMat[1, 1] == 0 && jacobiMat[0, 0] == 0 && jacobiMat[2, 2] == 0)
                {
                    //lambda2Values.InsertNextValue(0);
                    //lambda2ValuesList.Add(0);

                    count++;
                }
                //only if diagonal elements are not equal zero
                else
                {
                    MatrixOperations classObj = new MatrixOperations();

                    double[,] transposedJacobiMat = classObj.TransposeRowsAndColumns(jacobiMat);
                    // calculation of S und Omega
                    double[,] S = classObj.DivideArrayByValue(classObj.AddArrays(jacobiMat, transposedJacobiMat), 2);
                    double[,] squaredS = classObj.MultiplyArrays(S, S);

                    double[,] Omega = classObj.DivideArrayByValue(classObj.SubtractArrays(jacobiMat, transposedJacobiMat), 2);
                    double[,] squaredOmega = classObj.MultiplyArrays(Omega, Omega);

                    double[,] summation = classObj.AddArrays(squaredS, squaredOmega);

                    var eigenvalueDecomposition = new Accord.Math.Decompositions.EigenvalueDecomposition(summation, false, true, true);
                    var eigenVec = eigenvalueDecomposition.Eigenvectors;
                    var eigenValue = eigenvalueDecomposition.RealEigenvalues;
                    double lambda2 = eigenValue[1];

                    //int t = (int)lambda2Values.InsertNextValue(lambda2);

                    //lambda2ValuesList.Add(lambda2);
               
                    lambda2ValuesArray[(int)tetraPoints[k][0], 0] += lambda2;
                    lambda2ValuesArray[(int)tetraPoints[k][0], 1] += 1;
               
                  
                    lambda2ValuesArray[(int)tetraPoints[k][1], 0] += lambda2;
                    lambda2ValuesArray[(int)tetraPoints[k][1], 1] += 1;
                   
                    lambda2ValuesArray[(int)tetraPoints[k][2], 0] += lambda2;
                    lambda2ValuesArray[(int)tetraPoints[k][2], 1] += 1;
                 
                    lambda2ValuesArray[(int)tetraPoints[k][3], 0] += lambda2;
                    lambda2ValuesArray[(int)tetraPoints[k][3], 1] += 1;
                    
                    //if (k == 779834)
                    //    Console.WriteLine("Test");

                    //if (lambda2 < 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][0], 0] = lambda2;
                    //    lambda2ValuesArray[(int)tetraPoints[k][0], 1] += 1;
                    //}
                    ////else
                    ////{
                    ////    lambda2ValuesArray[(int)tetraPoints[k][0], 0] += lambda2;
                    ////    lambda2ValuesArray[(int)tetraPoints[k][0], 1] += 1;
                    ////}
                    //if (lambda2 < 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][1], 0] = lambda2;
                    //    lambda2ValuesArray[(int)tetraPoints[k][1], 1] += 1;

                    //}
                    ////else
                    ////{
                    ////    lambda2ValuesArray[(int)tetraPoints[k][1], 0] += lambda2;
                    ////    lambda2ValuesArray[(int)tetraPoints[k][1], 1] += 1;
                    ////}
                    //if (lambda2 < 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][2], 0] = lambda2;
                    //    lambda2ValuesArray[(int)tetraPoints[k][2], 1] += 1;

                    //}
                    ////else
                    ////{
                    ////    lambda2ValuesArray[(int)tetraPoints[k][2], 0] += lambda2;
                    ////    lambda2ValuesArray[(int)tetraPoints[k][2], 1] += 1;
                    ////}
                    //if (lambda2 < 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][3], 0] = lambda2;
                    //    lambda2ValuesArray[(int)tetraPoints[k][3], 1] += 1;

                    //}

                    // if (lambda2ValuesArray[(int)tetraPoints[k][0], 1] == 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][0], 0] = lambda2;  
                    //}
                    // if (lambda2ValuesArray[(int)tetraPoints[k][1], 1] == 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][1], 0] = lambda2;
                    //}
                    // if (lambda2ValuesArray[(int)tetraPoints[k][2], 1] == 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][2], 0] = lambda2;
                    //}
                    // if (lambda2ValuesArray[(int)tetraPoints[k][3], 1] == 0)
                    //{
                    //    lambda2ValuesArray[(int)tetraPoints[k][3], 0] = lambda2;
                    //}
                }
            }
            for(int i = 0; i < pointsToRegistrate.Count(); i++)
            {
                if (lambda2ValuesArray[i, 1] > 1)
                {
                    double lambda2 = lambda2ValuesArray[i, 0]/ lambda2ValuesArray[i, 1];
                    //if (lambda2 < 0)
                    //    Console.WriteLine("lambda2 ist " + lambda2);
                    lambda2ValuesList.Add(lambda2);
                    lambda2Values.InsertNextValue(lambda2);
                    count++;
                }
                else
                {
                    count++;
                    lambda2Values.InsertNextValue(lambda2ValuesArray[i, 0]);
                }
            }
            
            //WriteScalarDifferences(lambda2ValuesList, "..\\..\\..\\..\\..\\03Daten\\lambda2\\Tomo_lambda2regularGridsOwnEigen.txt");
            vtkDataArray dataArray;
            dataArray = lambda2Values;
            dataArray.SetName("lambda2");
            if (this.scalar_dataArray.ContainsKey("lambda2"))
                this.scalar_dataArray["lambda2"] = lambda2Values;

            else
            {
                this.scalar_dataArray.Add("lambda2", dataArray);
                this.arrayNames.Add("lambda2");
            }
        }
        

        private void WriteScalarDifferences(List<double> vxDiff, string textFile)
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

        

        private void WriteTransformationMatrix(List<Vector4d> matrix, string textFile)
        {
            string lineindex;
            using (StreamWriter sw = File.CreateText(textFile))
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    lineindex = matrix[i].ToString() ;

                    sw.WriteLine(lineindex);
                }

                sw.Flush();

                sw.Close();
            }
        }
    }

    
}
