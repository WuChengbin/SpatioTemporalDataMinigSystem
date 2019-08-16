using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    class YMatrix
    {
        int[,] oriData;
        int[,] data;
        int rowCount;
        int colCount;
        List<Point> result = new List<Point>();
        public YMatrix(int _rowCount,int _colCount)
        {
            rowCount = _rowCount;
            colCount = _colCount;
            data = new int[rowCount, colCount];
        }

        /// <summary>
        /// 将矩阵全部设置为一个值
        /// </summary>
        /// <param name="_value">设置的值</param>
        public void SetAllValue(int _value)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = _value;
            }
        }

        /// <summary>
        /// 将矩阵全部设置为指定值
        /// </summary>
        /// <param name="_value">设置的值</param>
        public void SetGiveValue()
        {
            data[0, 0] = 5;
            data[0, 1] = 8;
            data[0, 2] = 5;
            data[0, 3] = 3;
            data[0, 4] = 2;
            //data[0, 5] = 3;

            data[1, 0] = 4;
            data[1, 1] = 1;
            data[1, 2] = 8;
            data[1, 3] = 9;
            data[1, 4] = 4;
            //data[2, 5] = 7;

            data[2, 0] = 8;
            data[2, 1] = 2;
            data[2, 2] = 6;
            data[2, 3] = 2;
            data[2, 4] = 3;
            //data[2, 5] = 5;

            data[3, 0] = 8;
            data[3, 1] = 6;
            data[3, 2] = 6;
            data[3, 3] = 6;
            data[3, 4] = 3;
            //data[3, 5] = 6;

            data[4, 0] = 4;
            data[4, 1] = 6;
            data[4, 2] = 6;
            data[4, 3] = 1;
            data[4, 4] = 2;
            //data[4, 5] = 7;

            //data[5, 0] = 2;
            //data[5, 1] = 8;
            //data[5, 2] = 2;
            //data[5, 3] = 2;
            //data[5, 4] = 9;
            //data[5, 5] = 3;
        }

        /// <summary>
        /// 将矩阵全部设置为一随机值
        /// </summary>
        /// <param name="_value">设置的值</param>
        public void SetRandomValue(int minValue, int maxValue)
        {
            Random _r = new Random();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    data[i, j] = _r.Next(minValue, maxValue);
                }
            }
        }

        /// <summary>
        /// 设置一个值
        /// </summary>
        /// <param name="_r"></param>
        /// <param name="_c"></param>
        /// <param name="_value"></param>
        public void SetValue(int _r, int _c, int _value)
        {
            data[_r, _c] = _value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_r"></param>
        /// <param name="_c"></param>
        /// <returns></returns>
        public int GetOriValue(int _r, int _c)
        {
            return oriData[_r, _c];
        }

        public List<Point> GetResult()
        {
            return result;
        }

        public bool Calculation()
        {
            oriData = (int[,])data.Clone();//记录原始
            rowSub();//每行的所有数字减去该行的最小项
            colSub();//每列的所有数字减去该列的最小项
            while (true)
            {
                /*
                bool isGetResult = CalculateResult();//计算结果
                //if (!isGetResult) return false;
                if(result.Count== rowCount)
                {
                    return true;
                }
                else
                {
                    List<int> horLines;//横线id
                    List<int> verLines;//竖线id
                    int lineCount = GetLineMinCount(out horLines, out verLines);//使用横线或者竖线穿过矩阵中的所有0，并记录达成此目的所需的最少线路总数
                    MinValueSubAdd(horLines, verLines);
                }
                */

                
                List<int> horLines;//横线id
                List<int> verLines;//竖线id
                int lineCount = GetLineMinCountOld(out horLines, out verLines);//使用横线或者竖线穿过矩阵中的所有0，并记录达成此目的所需的最少线路总数
                if (lineCount > rowCount)
                {
                    int a = 0;
                }
                if (lineCount >= rowCount)
                {//存在解
                    bool isGetResult = CalculateResult();//计算结果
                    //lineCount = GetLineMinCount(out horLines, out verLines);
                    if (isGetResult)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    MinValueSubAdd(horLines, verLines);
                }
                
            }
        }

        /// <summary>
        /// 划线
        /// </summary>
        /// <param name="rightHorLines"></param>
        /// <param name="rightVerLines"></param>
        private void GetRightLines(out List<int> rightHorLines, out List<int> rightVerLines)
        {
            rightHorLines = new List<int>();//打对号的行
            rightVerLines = new List<int>();//打对号的列
            for(int rowId=0;rowId<rowCount;rowId++)
            {//each row
                if(result.Exists(_p => _p.X == rowId)) continue; //out this row
                else rightHorLines.Add(rowId);
                //for (int colId=0;colId<colCount;colId++)
                //{//each col
                //    if (data[rowId, colId] == 0)
                //    {//zero valu
                //        rightHorLines.Add(rowId);
                //        break; //out this row
                //    }
                //}
            }
            bool addRight; //Is add right?
            do
            {
                addRight = false;
                foreach (int rowId in rightHorLines)
                {
                    for (int colId = 0; colId < colCount; colId++)
                    { //each row
                        if (data[rowId, colId] == 0 && !rightVerLines.Exists(_l => _l == colId))
                        {
                            rightVerLines.Add(colId);
                            addRight = true;
                        }
                    }
                }
                foreach (int colId in rightVerLines)
                {
                    Point p = result.Find(_p => _p.Y == colId);
                    int rowId = p.X;
                    if (!rightHorLines.Exists(_l => _l == rowId))
                    {
                        rightHorLines.Add(rowId);
                        addRight = true;
                    }
                }
            } while (addRight == true);
            
        }

        /// <summary>
        /// 计算结果
        /// </summary>
        private bool CalculateResult()
        {
            #region 未完成的新方法
            /*
            result.Clear();
            List<int> horLines = new List<int>();//划掉的横线
            List<int> verLines = new List<int>();//划掉的竖线

            int zeroCountAll = 0;
            bool isExistRec = false;
            do
            {
                bool isLine = false;//是否增加新的划线
                do
                {
                    isLine = false;
                    for (int rowId = 0; rowId < rowCount; rowId++)
                    {//each row
                        if (horLines.Contains(rowId)) continue;//have line
                        int zeroCount = 0;
                        int zeroCol = -1;
                        for (int colId = 0; colId < colCount; colId++)
                        {//each col
                            if (data[rowId, colId] == 0 && !verLines.Exists(vl => vl == colId) && !result.Exists(r => (r.X == rowId) && (r.Y == colId)))
                            {//zero value; not have line; not in result
                                zeroCount++;
                                zeroCol = colId;
                            }
                            if (zeroCount > 1)
                            {//forward out
                                break;
                            }
                        }
                        if (zeroCount == 1)
                        {//only one zero value
                            verLines.Add(zeroCol);//add verLine
                            result.Add(new Point(rowId, zeroCol));
                            isLine = true;
                        }
                    }

                    for (int colId = 0; colId < colCount; colId++)
                    {
                        if (verLines.Contains(colId)) continue;
                        int zeroCount = 0;
                        int zeroRow = -1;
                        for (int rowId = 0; rowId < rowCount; rowId++)
                        {
                            if (data[rowId, colId] == 0 && !horLines.Exists(vl => vl == rowId) && !result.Exists(r => (r.X == rowId) && (r.Y == colId)))
                            {
                                zeroCount++;
                                zeroRow = rowId;
                            }
                            if (zeroCount > 1)
                            {
                                break;
                            }
                        }
                        if (zeroCount == 1)
                        {
                            horLines.Add(zeroRow);
                            result.Add(new Point(zeroRow, colId));
                            isLine = true;
                        }
                    }
                } while (isLine == true);

                zeroCountAll = GetZeroCount(horLines, verLines);
                if (zeroCountAll == 0) return true;
                isExistRec = false;//is exist rectangle
                if (zeroCountAll > 0)
                {
                    for (int i = 0; i < rowCount - 1; i++)
                    {//行
                        if (horLines.Contains(i)) continue;//该行已经划掉
                        for (int j = 0; j < colCount - 1; j++)
                        {//列
                            if (verLines.Contains(j)) continue;//该列已经划掉
                            if (data[i, j] == 0 && !result.Exists(r => (r.X == i) && (r.Y == j)))
                            {
                                //onePoint = new Point(i, j);
                                //寻找闭回路
                                
                            }
                        }
                    }
                }
            } while (zeroCountAll > 0 && isExistRec == true);
            return false;
            */
            #endregion

            #region 20180925
            
            result.Clear();
            List<int> horLines = new List<int>();//划掉的横线
            List<int> verLines = new List<int>();//划掉的竖线
            while (result.Count<rowCount)
            {//结果数目不对
                Point necessPoint;
                bool existNecess = GetNecessPoint(horLines, verLines,out necessPoint);
                if(existNecess)
                {//找到了
                    result.Add(necessPoint);
                    horLines.Add(necessPoint.X);//划掉该行
                    verLines.Add(necessPoint.Y);//划掉该列
                }
                else
                {//没有找到必须的
                    Point onePoint;
                    bool existPoint= GetOnePoint(horLines, verLines,out onePoint);
                    if(existPoint)
                    {
                        result.Add(onePoint);
                        horLines.Add(onePoint.X);//划掉该行
                        verLines.Add(onePoint.Y);//划掉该列
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
            
            #endregion

            #region
            //List<Point> oneLineZeroPoints = new List<Point>();//只有一条线穿过的零值
            //List<Point> twoLineZeroPoints = new List<Point>();//两条线穿过的零值
            //for(int i=0;i<rowCount;i++)
            //{//行
            //    for(int j=0;j<colCount;j++)
            //    {//列
            //        if(data[i,j]==0)
            //        {//零值
            //            if (horLines.Contains(i) && verLines.Contains(j)) twoLineZeroPoints.Add(new Point(i, j));//两线穿过的点
            //            else oneLineZeroPoints.Add(new Point(i, j));//一线穿过的点
            //        }
            //    }
            //}

            //while(result.Count<rowCount)
            //{//结果不够
            //    Jump1:
            //    foreach(int rowNo in horLines)
            //    {//横线
            //        for(int colNo=0;colNo<colCount;colNo++)
            //        {//每列
            //            if(data[rowNo,colNo]==0&&!verLines.Contains(colNo))
            //            {//一条横线穿过的零值
            //                result.Add(new Point(rowNo, colNo));
            //                horLines.Remove(rowNo);
            //                goto Jump1;//重新遍历横线
            //            }
            //        }
            //    }
            //    Jump2:
            //    foreach (int colNo in verLines)
            //    {//竖线
            //        for(int rowNo=0;rowNo<rowCount;rowNo++)
            //        {//每行
            //            if(data[rowNo,colNo]==0&&!horLines.Contains(rowNo))
            //            {//一条竖线穿过的零值
            //                result.Add(new Point(rowNo, colNo));
            //                verLines.Remove(colNo);
            //                goto Jump2;//重新遍历竖线
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        private int GetZeroCount(List<int> horLines, List<int> verLines)
        {
            int zeroCount = 0;
            for(int rowId=0;rowId<rowCount;rowId++)
            {//each row
                if (horLines.Contains(rowId)) continue;//forward out
                for(int colId=0;colId<colCount;colId++)
                {//each col
                    if(data[rowId,colId]==0&&!verLines.Contains(colId)&&!result.Exists(r => (r.X == rowId) && (r.Y == colId)))
                    {//zero value; not int line; not in result
                        zeroCount++;
                    }
                }
            }
            return zeroCount;
        }

        /// <summary>
        /// 返回第一个符合条件的
        /// </summary>
        /// <param name="horLines"></param>
        /// <param name="verLines"></param>
        /// <returns></returns>
        private bool GetOnePoint(List<int> horLines, List<int> verLines,out Point onePoint)
        {
            onePoint = new Point();
            for (int i = 0; i < rowCount-1; i++)
            {//行
                if (horLines.Contains(i)) continue;//该行已经划掉
                for (int j = 0; j < colCount-1; j++)
                {//列
                    if (verLines.Contains(j)) continue;//该列已经划掉
                    if (data[i, j] == 0)
                    {
                        onePoint = new Point(i, j);
                        return true;
                        ////寻找矩形
                        //for (int rowId=i+1;rowId<rowCount;rowId++)
                        //{
                        //    if(data[rowId, j] == 0)
                        //    {
                        //        for(int colId=j+1;colId<colCount;colId++)
                        //        {
                        //            if(data[i,colId]==0&&data[rowId,colId]==0)
                        //            {
                        //                onePoint = new Point(i, j);
                        //                return true;
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取必须选择的点
        /// </summary>
        /// <param name="horLines"></param>
        /// <param name="verLines"></param>
        /// <returns></returns>
        private bool GetNecessPoint(List<int> horLines, List<int> verLines,out Point necessPoint)
        {
            necessPoint = new Point();
            for (int i=0;i<rowCount;i++)
            {//行
                if (horLines.Contains(i)) continue;//该行已经划掉
                for (int j=0;j<colCount;j++)
                {//列
                    if (verLines.Contains(j)) continue;//该列已经划掉
                    if(data[i,j]==0)
                    {//零值
                        bool horNecess = true;//横向是否必须
                        for (int colNo = 0; colNo < colCount; colNo++)
                        {//该点所在行
                            if (data[i, colNo] == 0 && !verLines.Contains(colNo) && colNo != j)
                            {//横向存在其他没有划线的零
                                horNecess = false;
                                break;//不需要在判断了
                            }
                        }
                        if (horNecess)
                        {//横向必须
                            necessPoint = new Point(i, j);
                            return true;//找到
                        }
                        else
                        {
                            bool verNecess = true;//竖向是否必须
                            for (int rowNo = 0; rowNo < rowCount; rowNo++)
                            {//该点所在列
                                if (data[rowNo, j] == 0 && !horLines.Contains(rowNo) && rowNo != i)
                                {//竖向存在其他没有划线的零
                                    verNecess = false;
                                    break;//不需要在判断后面的了
                                }
                            }
                            if (verNecess)
                            {//竖向必须
                                necessPoint = new Point(i, j);
                                return true;//找到
                            }
                        }
                    }
                }
            }
            return false;//没有找到
        }

        /// <summary>
        /// 每行的所有数字减去该行的最小项
        /// </summary>
        private void rowSub()
        {
            for(int i=0;i<rowCount;i++)
            {//每行
                int minValue = GetRowMinValue(i);//获取该行最小值
                RowSubValue(i, minValue);//该行减去最小值
            }
        }

        /// <summary>
        /// 每列的所有数字减去该列的最小项
        /// </summary>
        private void colSub()
        {
            for (int j = 0; j < colCount; j++)
            {//每行
                int minValue = GetColMinValue(j);//获取该行最小值
                ColSubValue(j, minValue);//该行减去最小值
            }
        }

        /// <summary>
        /// 寻找行中最小值
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int GetRowMinValue(int i)
        {
            int minValue = data[i, 0];
            for(int j=0;j<colCount;j++)
            {//每一列
                if (data[i, j] < minValue) minValue = data[i, j];//寻找最小值
            }
            return minValue;
        }

        /// <summary>
        /// 寻找行中最小值
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private int GetColMinValue(int j)
        {
            int minValue = data[0, j];
            for (int i = 0; i < rowCount; i++)
            {//每一行
                if (data[i, j] < minValue) minValue = data[i, j];//寻找最小值
            }
            return minValue;
        }

        /// <summary>
        /// 行中所有元素减去一个值
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        private void RowSubValue(int i, int value)
        {
            for(int j=0;j<colCount;j++)
            {//每列
                data[i, j] -= value;
            }
        }

        /// <summary>
        /// 列中所有元素减去一个值
        /// </summary>
        /// <param name="j"></param>
        /// <param name="value"></param>
        private void ColSubValue(int j, int value)
        {
            for (int i = 0; i < rowCount; i++)
            {//每行
                data[i, j] -= value;
            }
        }
        
        private int GetLineMinCount(out List<int> horLines, out List<int> verLines)
        {
            List<int> rightHorLines;//打对号的行
            List<int> rightVerLines;//打对号的列
            GetRightLines(out rightHorLines, out rightVerLines);//打对号

            horLines = new List<int>();//横线id
            verLines = rightVerLines;//竖线id
            for (int rowId = 0; rowId < rowCount; rowId++)
            { //each row
                if (!rightHorLines.Exists(_l => _l == rowId))
                {
                    horLines.Add(rowId);
                }
            }
            return horLines.Count + verLines.Count;
        }

        #region 以前的20180828

        private int GetLineMinCountOld(out List<int> horLines,out List<int> verLines)
        {
            horLines = new List<int>();//横线
            verLines = new List<int>();//竖线

            int[] horZeroCount;//横向0个数
            int[] verZeroCount;//纵向0个数
            int zeroCount = GetZeroCount(horLines, verLines, out horZeroCount, out verZeroCount);//未被划线零个数
            while(zeroCount!=0)
            {//存在未被划线的零值
                int horMaxValue = -1;//横线零数最大值
                int horMaxValuePos = -1;//横线零数最大值位置
                for (int rowId = 0; rowId < rowCount; rowId++)
                {//每行
                    if (horZeroCount[rowId] > horMaxValue)
                    {//更多的零
                        horMaxValue = horZeroCount[rowId];
                        horMaxValuePos = rowId;
                    }
                }

                int verMaxValue = -1;//竖线零数最大值
                int verMaxValuePos = -1;//竖线零数最大值位置
                for (int colId = 0; colId < colCount; colId++)
                {//每列
                    if (verZeroCount[colId] > verMaxValue)
                    {//更多的零
                        verMaxValue = verZeroCount[colId];
                        verMaxValuePos = colId;
                    }
                }

                if (horMaxValue > verMaxValue)
                {//零数最多存在于横向
                    horLines.Add(horMaxValuePos);
                }
                else
                {
                    verLines.Add(verMaxValuePos);
                }

                zeroCount = GetZeroCount(horLines, verLines, out horZeroCount, out verZeroCount);//未被划线零个数
            }

            return horLines.Count + verLines.Count;

            #region 以前的20180827
            
            //horLines = new List<int>();//横线
            //verLines = new List<int>();//竖线
            //for(int i=0;i<rowCount;i++)
            //{//每行
            //    for(int j=0;j<colCount;j++)
            //    {//每列
            //        if (horLines.Contains(i)) break;//该行已经划线，跳出行循环
            //        if (verLines.Contains(j)) continue;//该列已经划线，结束本次处理
            //        if(data[i,j]==0)
            //        {//当前位置为零
            //            bool lineDir = GetLineDir(i,j, horLines, verLines);//获取线方向，true为横线，false为竖线
            //            if (lineDir) horLines.Add(i);//添加横线
            //            else verLines.Add(j);//添加竖线
            //        }
            //    }
            //}

            //List<int> removeLines = new List<int>();
            //foreach(int horLine in horLines)
            //{//每条横线
            //    bool canRemove = true;//是否可以移除
            //    for(int j=0;j<colCount;j++)
            //    {//每一列
            //        if(data[horLine, j] == 0 && !verLines.Contains(j))
            //        {//该位置没有竖线
            //            canRemove = false;//不能移除唯一的这条横线
            //            break;
            //        }
            //    }
            //    if (canRemove)
            //    {
            //        removeLines.Add(horLine);//记录移除
            //        //horLines.Remove(horLine);//移除该条线
            //    }
            //}
            //foreach(int horLine in removeLines)
            //{
            //    horLines.Remove(horLine);//移除该条线
            //}

            //removeLines.Clear();
            //foreach (int verLine in verLines)
            //{//每条竖线
            //    bool canRemove = true;//是否可以移除
            //    for (int i = 0; i < rowCount; i++)
            //    {//每一行
            //        if (data[i, verLine]==0&&!horLines.Contains(i))
            //        {//该位置没有横线
            //            canRemove = false;//不能移除唯一的这条竖线
            //            break;
            //        }
            //    }
            //    if (canRemove)
            //    {
            //        removeLines.Add(verLine);//记录移除
            //        //verLines.Remove(verLine);//移除该条线
            //    }
            //}
            //foreach (int verLine in removeLines)
            //{
            //    verLines.Remove(verLine);//移除该条线
            //}

            //int lineCount = horLines.Count + verLines.Count;//线的数目
            //return lineCount;
            
            #endregion
        }
    
        #endregion

        #region
        
        private int GetLineMinCountOld2(ref List<int> horLines, ref List<int> verLines)
        {
            int[] horZeroCount;//横向0个数
            int[] verZeroCount;//纵向0个数
            int zeroCount = GetZeroCount(horLines, verLines, out horZeroCount, out verZeroCount);//未被划线零个数
            if (zeroCount == 0) return 0;
            else
            {//存在未被划线的零
                int horMaxValue = -1;//横线零数最大值
                //int horMaxValuePos = -1;//横线零数最大值位置
                List<int> horMaxValuePosList = new List<int>();
                for (int rowId = 0; rowId < rowCount; rowId++)
                {//每行
                    if (horZeroCount[rowId] > horMaxValue)
                    {//更多的零
                        horMaxValue = horZeroCount[rowId];
                        horMaxValuePosList.Clear();
                        horMaxValuePosList.Add(rowId);
                    }
                    else if(horZeroCount[rowId] == horMaxValue)
                    {//相同的零
                        horMaxValuePosList.Add(rowId);
                    }
                }

                int verMaxValue = -1;//竖线零数最大值
                //int verMaxValuePos = -1;//竖线零数最大值位置
                List<int> verMaxValuePosList = new List<int>();
                for (int colId = 0; colId < colCount; colId++)
                {//每列
                    if (verZeroCount[colId] > verMaxValue)
                    {//更多的零
                        verMaxValue = verZeroCount[colId];
                        verMaxValuePosList.Clear();
                        verMaxValuePosList.Add(colId);
                    }
                    else if(verZeroCount[colId] == verMaxValue)
                    {//相同的零
                        verMaxValuePosList.Add(colId);
                    }
                }

                if (horMaxValue > verMaxValue)
                {//零数最多存在于横向
                    int minLinesCount = int.MaxValue;
                    List<int> horLinesCopyMinLinesCount=new List<int>();
                    foreach (int horMaxValuePos in horMaxValuePosList)
                    {
                        List<int> horLinesCopy = new List<int>(horLines);
                        horLinesCopy.Add(horMaxValuePos);
                        GetLineMinCountOld2(ref horLinesCopy, ref verLines);
                        int linesCount = horLinesCopy.Count + verLines.Count;
                        if(linesCount<minLinesCount)
                        {
                            minLinesCount = linesCount;
                            horLinesCopyMinLinesCount = horLinesCopy;
                        }
                    }
                    horLines = horLinesCopyMinLinesCount;
                }
                else if (horMaxValue < verMaxValue)
                {
                    int minLinesCount = int.MaxValue;
                    List<int> verLinesCopyMinLinesCount = new List<int>();
                    foreach(int verMaxValuePos in verMaxValuePosList)
                    {
                        List<int> verLinesCopy = new List<int>(verLines);
                        verLinesCopy.Add(verMaxValuePos);
                        GetLineMinCountOld2(ref horLines, ref verLinesCopy);
                        int linesCount = horLines.Count + verLinesCopy.Count;
                        if (linesCount < minLinesCount)
                        {
                            minLinesCount = linesCount;
                            verLinesCopyMinLinesCount = verLinesCopy;
                        }
                    }
                    verLines = verLinesCopyMinLinesCount;
                }
                else
                {//相等
                    //横线
                    int horMinLinesCount = int.MaxValue;
                    List<int> horLinesCopyMin1 = new List<int>();
                    List<int> verLinesCopyMin1 = new List<int>();
                    foreach (int horMaxValuePos in horMaxValuePosList)
                    {
                        List<int> horLinesCopy = new List<int>(horLines);
                        List<int> verLinesCopy = new List<int>(verLines);
                        horLinesCopy.Add(horMaxValuePos);
                        GetLineMinCountOld2(ref horLinesCopy, ref verLinesCopy);
                        int linesCount = horLinesCopy.Count + verLinesCopy.Count;
                        if (linesCount < horMinLinesCount)
                        {
                            horMinLinesCount = linesCount;
                            horLinesCopyMin1 = horLinesCopy;
                            verLinesCopyMin1 = verLinesCopy;
                        }
                    }

                    //竖线
                    int verMinLinesCount = int.MaxValue;
                    List<int> horLinesCopyMin2 = new List<int>();
                    List<int> verLinesCopyMin2 = new List<int>();
                    foreach (int verMaxValuePos in verMaxValuePosList)
                    {
                        List<int> horLinesCopy = new List<int>(horLines);
                        List<int> verLinesCopy = new List<int>(verLines);
                        verLinesCopy.Add(verMaxValuePos);
                        GetLineMinCountOld2(ref horLinesCopy, ref verLinesCopy);
                        int linesCount = horLinesCopy.Count + verLinesCopy.Count;
                        if (linesCount < verMinLinesCount)
                        {
                            verMinLinesCount = linesCount;
                            horLinesCopyMin2 = horLinesCopy;
                            verLinesCopyMin2 = verLinesCopy;
                        }
                    }
                    
                    //判断
                    if(horMinLinesCount < verMinLinesCount)
                    {
                        horLines = horLinesCopyMin1;
                        verLines = verLinesCopyMin1;
                    }
                    else
                    {
                        horLines = horLinesCopyMin2;
                        verLines = verLinesCopyMin2;
                    }
                }
                return horLines.Count + verLines.Count;
            }
        }
        
        #endregion

        /// <summary>
        /// 获取未被划线0个数
        /// </summary>
        /// <param name="horLines"></param>
        /// <param name="verLines"></param>
        /// <param name="horZeroCount"></param>
        /// <param name="verZeroCount"></param>
        private int GetZeroCount(List<int> horLines, List<int> verLines, out int[] horZeroCount, out int[] verZeroCount)
        {
            int zeroCount = 0;
            horZeroCount = new int[rowCount];
            verZeroCount = new int[colCount];
            for (int rowId = 0; rowId < rowCount; rowId++)
            {//每行
                for (int colId = 0; colId < colCount; colId++)
                {
                    if (data[rowId, colId] == 0 && !horLines.Contains(rowId) && !verLines.Contains(colId))
                    {
                        horZeroCount[rowId]++;
                        verZeroCount[colId]++;
                        zeroCount++;
                    }
                }
            }
            return zeroCount;
        }

        /// <summary>
        /// 计算划线方向，以划过0值较多为条件
        /// </summary>
        /// <param name="i">行</param>
        /// <param name="j">列</param>
        /// <returns>true为横线，false为竖线</returns>
        private bool GetLineDir(int i, int j,List<int> horLines,List<int> verLines)
        {
            int horZeroCount = 0;//横向0值个数
            int verZeroCount = 0;//竖向0值个数
            for (int colNo = 0; colNo < colCount; colNo++)
            {//判断横向
                if (data[i, colNo] == 0 && !verLines.Contains(colNo)) horZeroCount++;
            }
            for(int rolNo=0;rolNo<rowCount;rolNo++)
            {//判断纵向
                if ( data[rolNo, j] == 0 && !horLines.Contains(rolNo)) verZeroCount++;
            }
            bool lineDir = true;//划线方向
            if (verZeroCount > horZeroCount) lineDir = false;
            return lineDir;
        }

        /// <summary>
        /// 找到线路未覆盖的地方的最小项，存在未覆盖的项的行减去该项，然后将该项添加到覆盖的列中
        /// </summary>
        /// <param name="horLines"></param>
        /// <param name="verLines"></param>
        private void MinValueSubAdd(List<int> horLines, List<int> verLines)
        {
            int minNoLineValue = GetMinNoLineValue(horLines, verLines);//找到线路未覆盖的地方的最小项
            NoLineRowSubValue(horLines, verLines,minNoLineValue);//存在未覆盖的项的行减去该项
            VerLinesAddValue(horLines, verLines, minNoLineValue);//然后将该项添加到覆盖的列中
        }

        /// <summary>
        /// 然后将该项添加到覆盖的列中
        /// </summary>
        /// <param name="horLines"></param>
        /// <param name="verLines"></param>
        /// <param name="minNoLineValue"></param>
        private void VerLinesAddValue(List<int> horLines, List<int> verLines, int minNoLineValue)
        {
            if(verLines.Count!=0)
            {//存在竖线
                foreach(int colNo in verLines)
                {//每条竖线
                    for(int rowNo=0;rowNo<rowCount;rowNo++)
                    {//每行
                        data[rowNo, colNo] += minNoLineValue;
                    }
                }
            }
        }

        /// <summary>
        /// 存在未覆盖的项的行减去该项
        /// </summary>
        /// <param name="minNoLineValue"></param>
        private void NoLineRowSubValue(List<int> horLines, List<int> verLines, int minNoLineValue)
        {
            for(int i=0;i<rowCount;i++)
            {//行
                if(!horLines.Contains(i)&&verLines.Count!=colCount)
                {//该行存在没有被覆盖的值
                    for (int j = 0; j < colCount; j++) data[i, j] -= minNoLineValue;//减去最小值
                }
            }
        }

        /// <summary>
        /// 寻找没有被划线位置最小值
        /// </summary>
        /// <param name="horLines"></param>
        /// <param name="verLines"></param>
        /// <returns></returns>
        private int GetMinNoLineValue(List<int> horLines, List<int> verLines)
        {
            int minValue = int.MaxValue;//最小值
            for(int i=0;i<rowCount;i++)
            {//行
                if (horLines.Contains(i)) continue;//当前行为划线行，跳出本次循环
                for(int j=0;j<colCount;j++)
                {//列
                    if (verLines.Contains(j)) continue;//当前列为划线列，跳出本次循环
                    if (data[i, j] < minValue) minValue = data[i, j];
                }
            }
            return minValue;
        }
    }
}
