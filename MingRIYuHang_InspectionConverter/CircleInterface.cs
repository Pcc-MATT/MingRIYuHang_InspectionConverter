using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MingRIYuHang_InspectionConverter
{
   public class CircleInterface
    {
        public List<string> pointModelFirstFileContent;
        public List<string> pointModelSecondFileContent;
        public List<string> pointModelFirstContentList = new List<string>();
        public List<string> pointModelSecondContentList = new List<string>();
        /// <summary>
        /// get point model file content
        /// </summary>
        /// <param name="pointModelFilePath">point model file path</param>
        /// <param name="pointModelContent">point model content</param>
        /// <returns></returns>
        public List<string> readPointModelFile(string pointModelFilePath, List<string> pointModelContent)
        {
            if (File.Exists(pointModelFilePath))
            {
                pointModelContent = new List<string>();
                StreamReader sr = new StreamReader(pointModelFilePath);
                string oneLine;
                while ((oneLine = sr.ReadLine()) != null)
                {
                    pointModelContent.Add(oneLine);
                }
                sr.Close();
                return pointModelContent;
            }
            else { return null; }

        }
        /// <summary>
        /// insert data to point model file
        /// </summary>
        /// <param name="point">point data</param>
        /// <param name="identifierName">identifier Name</param>
        public void insertData2PointModelFile(Circle circle, string identifierName)
        {
            if (readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\CircleModelFirstContent.dat", pointModelFirstFileContent) != null)
            {
                pointModelFirstFileContent = readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\CircleModelFirstContent.dat", pointModelFirstFileContent);
            }
            if (readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\CircleModelSecondContent.dat", pointModelSecondFileContent) != null)
            {
                pointModelSecondFileContent = readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\CircleModelSecondContent.dat", pointModelSecondFileContent);
            }
            List<string> tempPointModelFirstFileContent = new List<string>();
            List<string> tempPointModelSecondFileContent = new List<string>();
            string firstTemp1 = "";
            string secTemp1 = "";
            foreach (string firstTemp in pointModelFirstFileContent)
            {
                firstTemp1 = firstTemp;
                if (firstTemp.Contains("CylinderIndex"))
                {
                    firstTemp1 = firstTemp.Replace("CylinderIndex", identifierName);
                }
                if (firstTemp.Contains("nominalXValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalXValue", circle.X.ToString("f4"));
                }
                if (firstTemp.Contains("nominalYValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalYValue", circle.Y.ToString("f4"));
                }
                if (firstTemp.Contains("nominalZValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalZValue", circle.Z.ToString("f4"));
                }
                if (firstTemp.Contains("nominalIValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalIValue", circle.I.ToString("f4"));
                }
                if (firstTemp.Contains("nominalJValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalJValue", circle.J.ToString("f4"));
                }
                if (firstTemp.Contains("nominalKValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalKValue", circle.K.ToString("f4"));
                }
                if (firstTemp.Contains("nominalRadius"))
                {
                    firstTemp1 = firstTemp.Replace("nominalRadius", (circle.diameter/2).ToString("f4"));
                }
                if (firstTemp.Contains("circleType"))
                {
                    if (circle.type == "outterCircle")
                    {
                        firstTemp1 = firstTemp.Replace("circleType", "1");
                    }
                    else {
                        firstTemp1 = firstTemp.Replace("circleType", "-1");
                    }
                }
                tempPointModelFirstFileContent.Add(firstTemp1);
            }
            foreach (string secTemp in pointModelSecondFileContent)
            {
                secTemp1 = secTemp;
                if (secTemp.Contains("circleName"))
                {
                    secTemp1 = secTemp.Replace("circleName", circle.name);
                }                
                if (secTemp.Contains("CylinderIndex"))
                {
                    secTemp1 = secTemp.Replace("CylinderIndex", identifierName);
                }
                if (secTemp.Contains("saftyDirection"))
                {
                    secTemp1 = secTemp.Replace("saftyDirection", "SP +Z");
                }
                tempPointModelSecondFileContent.Add(secTemp1);
            }
            pointModelFirstFileContent = tempPointModelFirstFileContent;
            pointModelSecondFileContent = tempPointModelSecondFileContent;
        }

        public void combineCircleModel(Circle pt, string identifierName)
        {
            insertData2PointModelFile(pt, identifierName);
            pointModelFirstContentList.AddRange(pointModelFirstFileContent);
            pointModelSecondContentList.AddRange(pointModelSecondFileContent);
        }
    }
}
