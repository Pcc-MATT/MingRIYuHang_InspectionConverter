using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace calypsoInterface
{
    public class PointInterface
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
        public List<string> readPointModelFile(string pointModelFilePath,List<string> pointModelContent)
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
        public void insertData2PointModelFile(Point point,string identifierName)
        {
            if(readPointModelFile(System.Environment.CurrentDirectory+ @"\Resources\PointModelFirstContent.dat", pointModelFirstFileContent) != null)
            {
                pointModelFirstFileContent = readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\PointModelFirstContent.dat", pointModelFirstFileContent);
            }
            if (readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\PointModelSecondContent.dat", pointModelSecondFileContent) != null)
            {
                pointModelSecondFileContent = readPointModelFile(System.Environment.CurrentDirectory + @"\Resources\PointModelSecondContent.dat", pointModelSecondFileContent);
            }
            List<string> tempPointModelFirstFileContent = new List<string>();
            List<string> tempPointModelSecondFileContent = new List<string>();
            string firstTemp1="";
            string secTemp1="";
            foreach (string firstTemp in pointModelFirstFileContent)
            {
                firstTemp1 = firstTemp;
                if (firstTemp.Contains("identifierName"))
                {
                    firstTemp1=firstTemp.Replace("identifierName", identifierName);
                }
                if (firstTemp.Contains("nominalXValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalXValue", point.X.ToString("f4"));
                }
                if (firstTemp.Contains("nominalYValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalYValue", point.Y.ToString("f4"));
                }
                if (firstTemp.Contains("nominalZValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalZValue", point.Z.ToString("f4"));
                }
                if (firstTemp.Contains("nominalIValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalIValue", point.I.ToString("f4"));
                }
                if (firstTemp.Contains("nominalJValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalJValue", point.J.ToString("f4"));
                }
                if (firstTemp.Contains("nominalKValue"))
                {
                    firstTemp1 = firstTemp.Replace("nominalKValue", point.K.ToString("f4"));
                }

                tempPointModelFirstFileContent.Add(firstTemp1);
            }
            foreach(string secTemp in pointModelSecondFileContent)
            {
                secTemp1 = secTemp;
                if (secTemp.Contains("pointName"))
                {
                    secTemp1=secTemp.Replace("pointName", point.name);
                }
                if (secTemp.Contains("pointType"))
                {
                    secTemp1=secTemp.Replace("pointType", point.type);
                }
                if (secTemp.Contains("identifierName"))
                {
                    secTemp1=secTemp.Replace("identifierName", identifierName);
                }
                tempPointModelSecondFileContent.Add(secTemp1);
            }
            pointModelFirstFileContent = tempPointModelFirstFileContent;
            pointModelSecondFileContent = tempPointModelSecondFileContent;
        }

        public void combinePointModel(Point pt,string identifierName)
        {
            insertData2PointModelFile(pt, identifierName);
            pointModelFirstContentList.AddRange(pointModelFirstFileContent);
            pointModelSecondContentList.AddRange(pointModelSecondFileContent);
        }

    }
}
