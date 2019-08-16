using System;
using System.Collections.Generic;
using System.Drawing;

namespace MarineSTMiningSystem
{
    class ZMatrix
    {
        private int[,] _dataOri;
        private int[,] _data;
        private List<Point> _result = new List<Point>();
        private int _x;
        private int _y;

        public ZMatrix(int botNum, int PointNum)
        {
            _x = botNum;
            _y = PointNum;
            _data = new int[botNum, PointNum];
            _dataOri = new int[botNum, PointNum];
            //_data[0, 0] = 1;
            //_data[0, 1] = 0;
            //_data[0, 2] = 1;
            //_data[0, 3] = 1;

            //_data[1, 0] = 0;
            //_data[1, 1] = 1;
            //_data[1, 2] = 1;
            //_data[1, 3] = 1;

            //_data[2, 0] = 1;
            //_data[2, 1] = 1;
            //_data[2, 2] = 1;
            //_data[2, 3] = 0;

            //_data[3, 0] = 1;
            //_data[3, 1] = 1;
            //_data[3, 2] = 0;
            //_data[3, 3] = 1;
        }

        /// <summary>
        /// 将矩阵全部设置为一个值
        /// </summary>
        /// <param name="_value">设置的值</param>
        public void SetAllValue(int _value)
        {
            for(int i=0;i< _data.GetLength(0);i++)
            {
                for (int j = 0; j < _data.GetLength(1); j++)
                    _data[i, j] = _value;
            }
        }

        /// <summary>
        /// 将矩阵全部设置为指定值
        /// </summary>
        /// <param name="_value">设置的值</param>
        public void SetGiveValue()
        {
            _data[0, 0] = 6;
            _data[0, 1] = 9;
            _data[0, 2] = 8;
            _data[0, 3] = 8;
            _data[0, 4] = 1;
            _data[0, 5] = 3;

            _data[1, 0] = 7;
            _data[1, 1] = 7;
            _data[1, 2] = 6;
            _data[1, 3] = 3;
            _data[1, 4] = 9;
            _data[2, 5] = 7;

            _data[2, 0] = 1;
            _data[2, 1] = 7;
            _data[2, 2] = 1;
            _data[2, 3] = 9;
            _data[2, 4] = 6;
            _data[2, 5] = 5;

            _data[3, 0] = 7;
            _data[3, 1] = 8;
            _data[3, 2] = 2;
            _data[3, 3] = 9;
            _data[3, 4] = 7;
            _data[3, 5] = 6;

            _data[4, 0] = 1;
            _data[4, 1] = 6;
            _data[4, 2] = 6;
            _data[4, 3] = 5;
            _data[4, 4] = 6;
            _data[4, 5] = 7;

            _data[5, 0] = 2;
            _data[5, 1] = 8;
            _data[5, 2] = 2;
            _data[5, 3] = 2;
            _data[5, 4] = 9;
            _data[5, 5] = 3;
        }

        /// <summary>
        /// 将矩阵全部设置为一随机值
        /// </summary>
        /// <param name="_value">设置的值</param>
        public void SetRandomValue(int minValue, int maxValue)
        {
            Random _r = new Random();
            for (int i = 0; i < _data.GetLength(0); i++)
            {
                for (int j = 0; j < _data.GetLength(1); j++)
                {
                    _data[i, j] = _r.Next(minValue, maxValue);
                    _dataOri[i, j] = _r.Next(minValue, maxValue);
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
            _data[_r, _c] = _value;
        }

        public List<Point> GetResult()
        {
            return _result;
        }


        public void Calculation()
        {
            step1();
            while (!step2())
            {
                step3();
            }
        }

        /// <summary>
        /// 畫出最少數目的垂直與水平的刪除線來包含所有的零至少一次。
        /// </summary>
        private void step3()
        {
            bool[,] isDelete = new bool[_x, _y];
            for (int x = 0; x < _x; x++)
            {
                for (int y = 0; y < _y; y++)
                {
                    if (_data[x, y] == 0 && !isDelete[x, y])
                    {
                        int xc = 0;
                        int yc = 0;

                        //lie
                        for (int nx = 0; nx < _x; nx++)
                        {
                            if (nx != x && _data[nx, y] == 0)
                            {
                                xc++;
                            }
                        }

                        //hang
                        for (int ny = 0; ny < _y; ny++)
                        {
                            if (ny != y && _data[x, ny] == 0)
                            {
                                yc++;
                            }
                        }

                        if (xc > yc)
                        {
                            for (int xx = 0; xx < _x; xx++)
                            {
                                isDelete[xx, y] = true;
                            }
                        }
                        else
                        {
                            for (int yy = 0; yy < _y; yy++)
                            {
                                isDelete[x, yy] = true;
                            }
                        }
                    }
                }
            }

            //找出未被畫線的元素中之最小值 K
            int k = 99999;
            for (int x = 0; x < _x; x++)
            {
                for (int y = 0; y < _y; y++)
                {
                    if (!isDelete[x, y])
                    {
                        if (_data[x, y] < k)
                        {
                            k = _data[x, y];
                        }
                    }
                }
            }

            //將含有此些未被畫線的元素的各列所有元素減去K 
            for (int x = 0; x < _x; x++)
            {
                for (int y = 0; y < _y; y++)
                {
                    if (!isDelete[x, y])
                    {
                        for (int y1 = 0; y1 < _y; y1++)
                        {
                            _data[x, y1] -= k;
                        }
                        break;
                    }
                }
            }

            //若造成負值，則將該欄加上K (Step 4.2)。形成新矩陣後回到Step2
            for (int x = 0; x < _x; x++)
            {
                for (int y = 0; y < _y; y++)
                {
                    if (_data[x, y] < 0)
                    {
                        for (int x1 = 0; x1 < _x; x1++)
                        {
                            _data[x1, y] += k;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 檢驗各列，對碰上之第一個零，做記號，同列或同欄的其他零則畫X (由零較少的列先做，可不依順序)
        /// 
        /// 檢驗可否完成僅含零的完全指派，若不能，則false
        /// </summary>
        private bool step2()
        {
            _result.Clear();
            bool[,] isDelete = new bool[_x, _y];


            //零的数量由少到多
            List<ZZeroNode> zeroNodes = new List<ZZeroNode>();
            for (int x = 0; x < _x; x++)
            {
                int zeroNum = 0;
                for (int y = 0; y < _y; y++)
                {
                    if (_data[x, y] == 0)
                    {
                        zeroNum++;
                    }
                }
                if (zeroNum > 0)
                {
                    zeroNodes.Add(new ZZeroNode(x, zeroNum));
                }
            }
            zeroNodes.Sort(ZZeroNode.Cmp);

            //从零较少的行开始
            while (zeroNodes.Count > 0)
            {
                ZZeroNode node = zeroNodes[0];

                if (node.ZeroNum <= 0)
                {
                    zeroNodes.RemoveAt(0);
                }
                else
                {
                    for (int y = 0; y < _y; y++)
                    {
                        if (_data[node.X, y] == 0 && !isDelete[node.X, y])
                        {
                            _result.Add(new Point(node.X, y));
                            zeroNodes.RemoveAt(0);

                            //删除与该零在同一列的其他零
                            for (int xxx = 0; xxx < _x; xxx++)
                            {
                                if (_data[xxx, y] == 0)
                                {
                                    isDelete[xxx, y] = true;
                                    for (int i = 0; i < zeroNodes.Count; i++)
                                    {
                                        if (zeroNodes[i].X == xxx)
                                        {
                                            zeroNodes[i].ZeroNum--;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                zeroNodes.Sort(ZZeroNode.Cmp);
            }

            //return _result.Count == _x;
            if(_result.Count == _x)
            {
                return true;
            }
            else
            {
                _result.Clear();
                isDelete = new bool[_x, _y];

                //零的数量由少到多
                zeroNodes = new List<ZZeroNode>();
                for (int y = 0; y < _y; y++)
                {
                    int zeroNum = 0;
                    for (int x = 0; x < _x; x++)
                    {
                        if (_data[x, y] == 0)
                        {
                            zeroNum++;
                        }
                    }
                    if (zeroNum > 0)
                    {
                        zeroNodes.Add(new ZZeroNode(y, zeroNum));
                    }
                }
                zeroNodes.Sort(ZZeroNode.Cmp);

                //从零较少的行开始
                while (zeroNodes.Count > 0)
                {
                    ZZeroNode node = zeroNodes[0];

                    if (node.ZeroNum <= 0)
                    {
                        zeroNodes.RemoveAt(0);
                    }
                    else
                    {
                        for (int x = 0; x < _x; x++)
                        {
                            if (_data[x, node.X] == 0 && !isDelete[x, node.X])
                            {
                                _result.Add(new Point(node.X, x));
                                zeroNodes.RemoveAt(0);

                                //删除与该零在同一行的其他零
                                for (int yyy = 0; yyy < _y; yyy++)
                                {
                                    if (_data[x, yyy] == 0)
                                    {
                                        isDelete[x, yyy] = true;
                                        for (int i = 0; i < zeroNodes.Count; i++)
                                        {
                                            if (zeroNodes[i].X == yyy)
                                            {
                                                zeroNodes[i].ZeroNum--;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    zeroNodes.Sort(ZZeroNode.Cmp);
                }

                return _result.Count == _y;
            }
        }

        /// <summary>
        /// 在各列中找最小值，將該列中各元素檢去此值，對各行重複一次。
        /// </summary>
        private void step1()
        {
            //列
            for (int x = 0; x < _x; x++)
            {
                int minY = 99999;
                //找到每列最小的值
                for (int y = 0; y < _y; y++)
                {
                    if (_data[x, y] < minY)
                    {
                        minY = _data[x, y];
                    }
                }
                //让该列减去最小的值
                for (int y = 0; y < _y; y++)
                {
                    _data[x, y] -= minY;
                }
            }
            //行
            for (int y = 0; y < _y; y++)
            {
                int minX = 99999;
                //找到每列最小的值
                for (int x = 0; x < _x; x++)
                {
                    if (_data[x, y] < minX)
                    {
                        minX = _data[x, y];
                    }
                }
                //让该列减去最小的值
                for (int x = 0; x < _x; x++)
                {
                    _data[x, y] -= minX;
                }
            }
        }
    }

    class ZZeroNode
    {
        public int X;
        public int ZeroNum;

        public ZZeroNode(int x, int zeroNum)
        {
            X = x;
            ZeroNum = zeroNum;
        }

        public static int Cmp(ZZeroNode a, ZZeroNode b)
        {
            return a.ZeroNum.CompareTo(b.ZeroNum);
        }
    }
}
