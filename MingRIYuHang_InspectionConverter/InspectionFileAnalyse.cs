using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MingRIYuHang_InspectionConverter
{
   public  class InspectionFileAnalyse
    {
        List<string> orignalInspectionPlanContent;
        PointInterface pointInterface;
        List<string> pointFirstContent;
        List<string> pointSecondContent;
        public string inspectionPath;

        /// <summary>
        /// read Inspection file of inspection plan
        /// </summary>
        /// <param name="inspectionPath">path of inspection file</param>
        /// <returns></returns>
        public List<string> readInspectionFile()
        {            
            StreamReader sr = new StreamReader(inspectionPath, Encoding.GetEncoding("GB18030"));//gb2312
            orignalInspectionPlanContent = new List<string>();
            string oneLine;
            while((oneLine=sr.ReadLine()) != null)
            {
                orignalInspectionPlanContent.Add(oneLine);
            }
            sr.Close();
            return orignalInspectionPlanContent;
        }
        public void write2InspectionFile()
        {
            FileStream fs = new FileStream(inspectionPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs,Encoding.GetEncoding("GB18030"));
            foreach(string str in orignalInspectionPlanContent)
            {
                sw.WriteLine(str);
            }
            sw.Close();
        }
        public void insertData2InspectionFile(List<Point> pointList,bool creatNewInspectionFlag)
        {
            readInspectionFile();
            pointInterface = new PointInterface();
            int identifierNumber = 0;
            int oldPointNumber = 0;
            //第一处插入点的信息
            int controlFeatureIndex = orignalInspectionPlanContent.FindIndex(n=>n== "#controlFeatures: ");
            if (controlFeatureIndex != -1)
            {
                string sss = orignalInspectionPlanContent[controlFeatureIndex - 1];
                orignalInspectionPlanContent[controlFeatureIndex - 1] = orignalInspectionPlanContent[controlFeatureIndex - 1].Remove(orignalInspectionPlanContent[controlFeatureIndex - 1].LastIndexOf(')')-1, 2);//去掉双括号
                if (!creatNewInspectionFlag)
                {
                    //获取已有inspection里的点的序号
                    List<string> pointNameList = new List<string>();
                    pointNameList= orignalInspectionPlanContent.FindAll(n => n.Contains("#identifier: 'P")&&n.Substring(15,1)!="l");
                    if (pointNameList.Count() == 0)
                    {
                        oldPointNumber = 0;
                    }
                    else
                    {
                        Regex re = new Regex("(?<=\').*?(?=\')", RegexOptions.None);
                        MatchCollection mc = re.Matches(pointNameList[pointNameList.Count() - 1]);
                        foreach (Match ma in mc)
                        {
                            string ss = ma.Value.ToString();
                            oldPointNumber = int.Parse(System.Text.RegularExpressions.Regex.Replace(ss, @"[^0-9]+", ""));
                        }
                    }
                    //获取已有inspection里plane的序号
                    List<string> planeTemp = new List<string>();
                    planeTemp = orignalInspectionPlanContent.FindAll(n => n.Contains("#('Plane"));
                    Regex re1 = new Regex("(?<=\').*?(?=\')", RegexOptions.None);
                    MatchCollection mc1 = re1.Matches(planeTemp[planeTemp.Count() - 1]);
                    foreach (Match ma1 in mc1)
                    {
                        string ss = ma1.Value.ToString();
                        identifierNumber = int.Parse(System.Text.RegularExpressions.Regex.Replace(ss, @"[^0-9]+", ""));
                        break;
                    }
                    //identifierNumber = int.Parse(planeTemp[planeTemp.Count() - 1].Substring(planeTemp[planeTemp.Count() - 1].IndexOf('e') + 1, 1));
                    foreach (Point pt in pointList)
                    {
                        identifierNumber++;
                        oldPointNumber++;
                        pt.pointNo = oldPointNumber;
                        pt.name = "P" + oldPointNumber;
                        pointInterface.combinePointModel(pt, "Plane" + identifierNumber);
                    }
                }
                else
                {
                    foreach (Point pt in pointList)
                    {
                        identifierNumber++;
                        pointInterface.combinePointModel(pt, "Plane" + identifierNumber);
                    }
                }
                pointFirstContent = pointInterface.pointModelFirstContentList;
                pointFirstContent[pointFirstContent.Count() - 1] = pointFirstContent[pointFirstContent.Count() - 1] + "))";
                orignalInspectionPlanContent.InsertRange(controlFeatureIndex, pointFirstContent);
                //第二处 插入点的信息
                pointSecondContent = pointInterface.pointModelSecondContentList;
                if (!creatNewInspectionFlag)
                {
                    int defaultTechnologyIndex = orignalInspectionPlanContent.FindLastIndex(n => n.Contains("#defaultTechnology: '"));//orignalInspectionPlanContent.FindIndex(n => n.Contains("#defaultTechnology: '***')))"))
                    orignalInspectionPlanContent.InsertRange(defaultTechnologyIndex, pointSecondContent);
                }
                else
                {
                    int safetyGroupIndex = orignalInspectionPlanContent.FindIndex(n => n.Contains("#safetyGroups: "));//#safetyGroups:
                    orignalInspectionPlanContent[safetyGroupIndex - 1] = orignalInspectionPlanContent[safetyGroupIndex - 1].Remove(orignalInspectionPlanContent[safetyGroupIndex - 1].Count()-3,3);//移除safetyGroup上一句的双括号
                    pointSecondContent.RemoveAt(0);
                    pointSecondContent.Add("#defaultTechnology: '***'))) ");
                    orignalInspectionPlanContent.InsertRange(safetyGroupIndex, pointSecondContent);
                }             
                //写入Inspection文件里
                write2InspectionFile();
            }           
        }

    }
}
