using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    static class ResterToVector
    {
        /// <summary>
        /// 第一版本，弃用
        /// </summary>
        /// <param name="inPath"></param>
        /// <param name="outPath"></param>
        public static void TifToShp(string inPath,string outPath)
        {
            //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

            //打开hdf文件
            Dataset ds = Gdal.Open(inPath, Access.GA_ReadOnly);
            int col = ds.RasterXSize;//列数
            int row = ds.RasterYSize;//行数
            Band demband1 = ds.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息

            string[] metadatas = ds.GetMetadata("");//获取元数据
            double startLog = 70.0;//起始经度
            double startLat = 3.0;//起始维度
            double endLog = 140.0;//结束经度
            double endLat = 53.0;//结束维度
            double mScale = 1.0;//比例
            string dataType = "";
            string imgDate = "";
            string fillValue = "";
            double resolution = 0.1;//分辨率
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        mScale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "DataType":
                        dataType = mdArr[1];//起始经度
                        break;
                    case "ImageDate":
                        imgDate = mdArr[1];
                        break;
                    case "FillValue":
                        fillValue = mdArr[1];
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }

            double[] databuf = new double[row * col];//存储
            demband1.ReadRaster(0, 0, col, row, databuf, col, row, 0, 0);//读取数据
            double[,] dataArr = new double[row, col];//二维数组,多两行是为了方便处理边缘结点
            for (int i = 0; i < databuf.Length; i++)
            {
                int _rowNow = i / col;//行号
                int _colNow = i % col;//列号
                if (_rowNow == 0 || _colNow == 0 || _rowNow == dataArr.GetLength(0) - 1 || _colNow == dataArr.GetLength(1) - 1) continue;//边界点不添加
                dataArr[_rowNow, _colNow] = databuf[i];
            }


            //寻找结点
            List<Node> nodes = new List<Node>();//结点链表
            for (int i = 0; i < dataArr.GetLength(0)-1; i++)
            {//行循环
                for (int j = 0; j < dataArr.GetLength(1) - 1; j++)
                {
                    double ltv = dataArr[i, j];//左上角栅格值
                    double rtv = dataArr[i, j + 1];//右上角栅格值
                    double lbv = dataArr[i + 1, j];//左下角栅格值
                    double rbv = dataArr[i + 1, j + 1];//右下角栅格值
                    if (ltv != rtv && rtv == lbv && lbv == rbv)
                    {//左上角不同，左和上方向
                        string dir1 = "l";//左方向
                        string dir2 = "t";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 1;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (ltv > 0) node.power = (int)ltv; else node.power = (int)rtv;
                        nodes.Add(node);
                    }
                    else if (rtv != ltv && ltv == lbv && lbv == rbv)
                    {//右上角不同，上和右方向
                        string dir1 = "t";//左方向
                        string dir2 = "r";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 2;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rtv > 0) node.power = (int)rtv; else node.power = (int)ltv;
                        nodes.Add(node);
                    }
                    else if (lbv != ltv && ltv == rtv && rtv == rbv)
                    {//左下角不同，下和左方向
                        string dir1 = "b";//左方向
                        string dir2 = "l";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 3;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (lbv > 0) node.power = (int)lbv; else node.power = (int)ltv;
                        nodes.Add(node);
                    }
                    else if (rbv != ltv && ltv == rtv && rtv == lbv)
                    {//右下角不同，右和下方向
                        string dir1 = "r";//左方向
                        string dir2 = "b";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 4;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rbv > 0) node.power = (int)rbv; else node.power = (int)ltv;
                        nodes.Add(node);
                    }
                    else if (ltv == rbv && ltv != rtv && rtv == lbv)
                    {//对角相等，相邻不等
                        string dir1 = "n";//左方向
                        string dir2 = "n";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        if (ltv > 0) { node.type = 5; node.power = (int)ltv; }//左上角和右下角值大于零
                        else { node.type = 6; node.power = (int)rtv; }//右上角和左下角值大于零
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                }
            }

            List <Line> lines = new List<Line>();//所有线
            //结点连成线
            for(int i=0;i<nodes.Count;i++)
            {//寻找每一条线
                if (nodes[i].isUsed == true|| nodes[i].type>4) continue;//被使用，跳出本次循环
                Node headNode = nodes[i];//头结点
                headNode.isUsed = true;//记录被使用
                headNode.outDir = headNode.dir1;//记录出去的方向
                nodes[i] = headNode;//进行保存
                Line line = new Line();//新建一条线
                int minRow = headNode.row;
                int minCol = headNode.col;
                int maxRow = headNode.row;
                int maxCol = headNode.col;
                //line.nodes.Add(headNode);//
                List<Node> lineNodes = new List<Node>();//线中所有点
                lineNodes.Add(headNode);//将第一个点添加进去
                Node tailNode = headNode;//尾结点
                //Node nextNode = new Node();//用来记录找到的下一个结点
                do
                {
                    tailNode = GetNextNode(tailNode, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                    lineNodes.Add(tailNode);//将点添加进去
                } while (tailNode.id!=headNode.id);
                
                line.id = lines.Count;
                line.nodes = lineNodes;//进行保存
                line.minRow = minRow;
                line.minCol = minCol;
                line.maxRow = maxRow;
                line.maxCol = maxCol;
                line.type = 0;//默认外环
                line.power = headNode.power;//值
                lines.Add(line);
            }

            //线构成面
            List<Polygon> polygons = new List<Polygon>();//所有面
            for(int i=0;i<lines.Count;i++)
            {//循环每一条线
                if (lines[i].type != 0) continue;//不是外环，退出本次循环
                Polygon polygon = new Polygon();
                List<Line> pLines = new List<Line>();
                pLines.Add(lines[i]);//添加外环
                polygon.id = polygons.Count;
                polygon.minCol = lines[i].minCol;
                polygon.minRow = lines[i].minRow;
                polygon.maxCol = lines[i].maxCol;
                polygon.maxRow = lines[i].maxRow;
                polygon.power = lines[i].power;
                for (int j=i+1;j<lines.Count;j++)
                {//寻找该外环包含的内环，内环肯定在后面
                    //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
                    if (lines[j].minRow > lines[i].minRow && lines[j].minCol > lines[i].minCol&& lines[j].maxRow < lines[i].maxRow && lines[j].maxCol < lines[i].maxCol)
                    {//邻接矩形包含，进一步判断
                        //只要一个点在外环内，就是内环
                        bool isIn = IsInPolygonNew(lines[j].nodes[0], lines[i].nodes);//判断是否在外环里面
                        if(isIn)
                        {//内环
                            Line line = lines[j];//取出线
                            line.nodes.Reverse();//顺序反转
                            line.type = 1;
                            lines[j] = line;//保存修改后的
                            pLines.Add(line);//添加内环
                        }
                    }
                }
                polygon.lines = pLines;
                polygons.Add(polygon);
            }

            //构成多面


            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = outPath;
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "…";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);
            //接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
            oFieldName.SetWidth(10);
            olayer1.CreateField(oFieldName, 1);

            //创建x坐标字段
            FieldDefn oFieldValue = new FieldDefn("Value", FieldType.OFTReal);
            oFieldValue.SetWidth(10);
            oFieldValue.SetPrecision(8);
            olayer1.CreateField(oFieldValue, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
            oFieldMinLog.SetWidth(10);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(10);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(10);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(10);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //写入数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, polygon.id);
                oFeature.SetField(1, "storm");
                oFeature.SetField(2, polygon.power);

                //计算经纬度
                double minLog = startLog + (polygon.minCol+1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow-1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol+1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow-1) * resolution;//最大纬度

                oFeature.SetField(3, minLog);
                oFeature.SetField(4, minLat);
                oFeature.SetField(5, maxLog);
                oFeature.SetField(6, maxLat);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "POLYGON (";
                foreach(Line line in polygon.lines)
                {//写出多边形中每条线
                    polygonStr += "(";
                    foreach (Node node in line.nodes)
                    {
                        int _row = node.row;//点行号
                        int _col = node.col;//点列号

                        double log = startLog + (_col+1) * resolution;//经度
                        double lat = startLat + (row - _row-1) * resolution;//纬度

                        polygonStr += log.ToString() + " " + lat.ToString();
                        polygonStr += ",";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer1.Dispose();
            ds1.Dispose();
        }

        /// <summary>
        /// 新版本
        /// </summary>
        /// <param name="oriPath"></param>
        /// <param name="spPath"></param>
        /// <param name="idPath"></param>
        /// <param name="outPath"></param>
        public static void TifToShp(string oriPath, double valueScale, double timeCell, string spPath, string idPath, string outPath)
        {
            //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

            //打开hdf文件
            Dataset oriDs = Gdal.Open(oriPath, Access.GA_ReadOnly);
            Dataset spDs = Gdal.Open(spPath, Access.GA_ReadOnly);
            Dataset idDs = Gdal.Open(idPath, Access.GA_ReadOnly);
            int col = oriDs.RasterXSize;//列数
            int row = oriDs.RasterYSize;//行数
            Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
            Band spBand1 = spDs.GetRasterBand(1);//读取波段
            Band idBand1 = idDs.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            oriDs.GetGeoTransform(argout);//读取地理坐标信息

            string[] metadatas = oriDs.GetMetadata("");//获取元数据
            double startLog = 70.0;//起始经度
            double startLat = 3.0;//起始维度
            double endLog = 140.0;//结束经度
            double endLat = 53.0;//结束维度
            double mScale = 1.0;//比例
            string dataType = "";
            string imgDate = "";
            string fillValue = "";
            double resolution = 0.1;//分辨率

            string fileName = Path.GetFileName(oriPath);
            DateTime startTime = DateTime.ParseExact(fileName.Substring(0, 16), "yyyyMMdd-SHHmmss", System.Globalization.CultureInfo.CurrentCulture);//起始时间 


            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        mScale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "DataType":
                        dataType = mdArr[1];//起始经度
                        break;
                    case "ImageDate":
                        imgDate = mdArr[1];
                        break;
                    case "FillValue":
                        fillValue = mdArr[1];
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }

            double[] oriData = new double[row * col];//存储
            int[] spData = new int[row * col];//存储
            int[] idData = new int[row * col];//存储
            oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
            spBand1.ReadRaster(0, 0, col, row, spData, col, row, 0, 0);//读取数据
            idBand1.ReadRaster(0, 0, col, row, idData, col, row, 0, 0);//读取数据
            oriDs.Dispose();
            spDs.Dispose();
            idDs.Dispose();

            double[,] oriImg = new double[row, col];//二维数组，边缘不处理
            int[,] spImg = new int[row, col];//二维数组，边缘不处理
            int[,] idImg = new int[row, col];//二维数组，边缘不处理
            for (int i = 0; i < spData.Length; i++)
            {//将一维数组转换为二维，方便处理
                int _rowNow = i / col;//行号
                int _colNow = i % col;//列号
                if (_rowNow == 0 || _colNow == 0 || _rowNow == spImg.GetLength(0) - 1 || _colNow == spImg.GetLength(1) - 1) continue;//边界点不添加
                if (oriData[i] > 0.0)
                {
                    oriImg[_rowNow, _colNow] = oriData[i] * valueScale * mScale;//乘以系数
                }
                else
                {//将0和负值赋值为0
                    oriImg[_rowNow, _colNow] = 0.0;
                }
                if (spData[i] > 0)
                {
                    spImg[_rowNow, _colNow] = spData[i];
                }
                else
                {
                    spImg[_rowNow, _colNow] = 0;
                }
                if (idData[i] > 0) idImg[_rowNow, _colNow] = idData[i];
                else idImg[_rowNow, _colNow] = 0;
            }


            //寻找结点
            List<Node> nodes = new List<Node>();//结点链表
            for (int i = 0; i < spImg.GetLength(0) - 1; i++)
            {//行循环
                for (int j = 0; j < spImg.GetLength(1) - 1; j++)
                {
                    int ltv = spImg[i, j];//左上角栅格值
                    int rtv = spImg[i, j + 1];//右上角栅格值
                    int lbv = spImg[i + 1, j];//左下角栅格值
                    int rbv = spImg[i + 1, j + 1];//右下角栅格值
                    if (ltv != rtv && rtv == lbv && lbv == rbv)
                    {//左上角不同，左和上方向
                        string dir1 = "l";//左方向
                        string dir2 = "t";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 1;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (ltv > 0)
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        else
                        {
                            node.power = rtv;
                            node.stormID = idImg[i, j + 1];//右上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (rtv != ltv && ltv == lbv && lbv == rbv)
                    {//右上角不同，上和右方向
                        string dir1 = "t";//左方向
                        string dir2 = "r";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 2;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rtv > 0)
                        {
                            node.power = rtv;
                            node.stormID = idImg[i, j + 1];//右上角栅格值
                        }
                        else
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (lbv != ltv && ltv == rtv && rtv == rbv)
                    {//左下角不同，下和左方向
                        string dir1 = "b";//左方向
                        string dir2 = "l";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 3;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (lbv > 0)
                        {
                            node.power = lbv;
                            node.stormID = idImg[i + 1, j];//左下角栅格值

                        }
                        else
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (rbv != ltv && ltv == rtv && rtv == lbv)
                    {//右下角不同，右和下方向
                        string dir1 = "r";//左方向
                        string dir2 = "b";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 4;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rbv > 0)
                        {
                            node.power = rbv;
                            node.stormID = idImg[i + 1, j + 1];//右下角栅格值
                        }
                        else
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (ltv == rbv && ltv != rtv && rtv == lbv)
                    {//对角相等，相邻不等
                        string dir1 = "n";//左方向
                        string dir2 = "n";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        if (ltv > 0)
                        {//左上角和右下角值大于零
                            node.type = 5;
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        else
                        {//右上角和左下角值大于零
                            node.type = 6;
                            node.power = rtv;
                            node.stormID = idImg[i, j + 1];//右上角栅格值
                        }
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                }
            }

            List<Line> lines = new List<Line>();//所有线
            //结点连成线
            for (int i = 0; i < nodes.Count; i++)
            {//寻找每一条线
                if (nodes[i].isUsed == true || nodes[i].type > 4) continue;//被使用，跳出本次循环
                Node headNode = nodes[i];//头结点
                headNode.isUsed = true;//记录被使用
                headNode.outDir = headNode.dir1;//记录出去的方向
                nodes[i] = headNode;//进行保存
                Line line = new Line();//新建一条线
                int minRow = headNode.row;
                int minCol = headNode.col;
                int maxRow = headNode.row;
                int maxCol = headNode.col;
                //line.nodes.Add(headNode);//
                List<Node> lineNodes = new List<Node>();//线中所有点
                lineNodes.Add(headNode);//将第一个点添加进去
                Node tailNode = headNode;//尾结点
                //Node nextNode = new Node();//用来记录找到的下一个结点
                do
                {
                    tailNode = GetNextNode(tailNode, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                    lineNodes.Add(tailNode);//将点添加进去
                } while (tailNode.id != headNode.id);

                line.id = lines.Count;
                line.nodes = lineNodes;//进行保存
                line.minRow = minRow;
                line.minCol = minCol;
                line.maxRow = maxRow;
                line.maxCol = maxCol;
                line.type = 0;//默认外环
                line.power = headNode.power;//值
                line.stormID = headNode.stormID;//值
                lines.Add(line);
            }

            //线构成面
            List<Polygon> polygons = new List<Polygon>();//所有面
            for (int i = 0; i < lines.Count; i++)
            {//循环每一条线
                if (lines[i].type != 0) continue;//不是外环，退出本次循环
                Polygon polygon = new Polygon();
                List<Line> pLines = new List<Line>();
                pLines.Add(lines[i]);//添加外环
                polygon.id = polygons.Count;
                polygon.stormID = lines[i].stormID;
                polygon.minCol = lines[i].minCol;
                polygon.minRow = lines[i].minRow;
                polygon.maxCol = lines[i].maxCol;
                polygon.maxRow = lines[i].maxRow;
                polygon.power = lines[i].power;
                for (int j = i + 1; j < lines.Count; j++)
                {//寻找该外环包含的内环，内环肯定在后面
                    //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
                    if (lines[j].stormID != polygon.stormID) continue;//暴雨id不相同
                    if (lines[j].minRow > lines[i].minRow && lines[j].minCol > lines[i].minCol && lines[j].maxRow < lines[i].maxRow && lines[j].maxCol < lines[i].maxCol)
                    {//邻接矩形包含，进一步判断
                        //只要一个点在外环内，就是内环
                        bool isIn = IsInPolygonNew(lines[j].nodes[0], lines[i].nodes);//判断是否在外环里面
                        if (isIn)
                        {//内环
                            Line line = lines[j];//取出线
                            line.nodes.Reverse();//顺序反转
                            line.type = 1;
                            lines[j] = line;//保存修改后的
                            pLines.Add(line);//添加内环
                        }
                    }
                }
                polygon.lines = pLines;
                double avgRainfall = 0.0;//平均降雨量
                double volume = 0.0;//累计降雨量
                double maxRainfall = 0.0;//最大降雨量
                double minRainfall = double.MaxValue;//最小降雨量
                double area = 0.0;//面积
                for (int _row = polygon.minRow + 1; _row <= polygon.maxRow; _row++)
                {//行循环，行列号对应节点左上角栅格
                    double rasterStartLat = startLat + (row - _row - 1) * resolution;//栅格下边缘纬度
                    double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
                    double rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积
                    for (int _col = polygon.minCol + 1; _col <= polygon.maxCol; _col++)
                    {//列循环
                        if (idImg[_row, _col] == polygon.stormID)
                        {//当前相投id的暴雨对象
                            float _rowF = _row - 0.5f;//栅格行列号转为矢量行列号
                            float _colF = _col - 0.5f;
                            if ( IsInPolygonNew(_rowF, _colF, polygon.lines[0].nodes))
                            {//包含关系
                                area += rasterArea;//增加面积
                                                   //double rainfall = oriImg[_row, _col] / timeCell;//换算为每小时
                                double rainfall = oriImg[_row, _col];//每半小时
                                if (rainfall < 0.0) rainfall = 0;
                                if (rainfall > maxRainfall) maxRainfall = rainfall;//最大降雨量
                                if (rainfall < minRainfall) minRainfall = rainfall;
                                volume += rasterArea * rainfall;//暴雨总降雨量累加
                            }
                        }
                    }
                }
                avgRainfall = volume / area;//计算平均降雨量
                polygon.avgRainfall = avgRainfall;//保存平均降雨量
                polygon.maxRainfall = maxRainfall;//保存最大降雨量
                polygon.minRainfall = minRainfall;//保存最大降雨量
                polygon.area = area;

                volume = volume * 1000;//立方米
                polygon.volume = volume;
                polygons.Add(polygon);
            }

            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = outPath;
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);
            //接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("StormID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStateID = new FieldDefn("StateID", FieldType.OFTString);
            oFieldStateID.SetWidth(20);
            olayer1.CreateField(oFieldStateID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
            oFieldName.SetWidth(20);
            olayer1.CreateField(oFieldName, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldTime.SetWidth(20);
            olayer1.CreateField(oFieldTime, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
            oFieldMinLog.SetWidth(20);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建area字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            olayer1.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgRain", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            //创建体积字段
            FieldDefn oFieldVolume = new FieldDefn("Volume", FieldType.OFTReal);
            oFieldVolume.SetWidth(20);
            oFieldVolume.SetPrecision(8);
            olayer1.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("MaxRain", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMaxRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("MinRain", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMinRainFall, 1);

            //创建Power字段
            FieldDefn oFieldValue = new FieldDefn("Power", FieldType.OFTReal);
            oFieldValue.SetWidth(20);
            oFieldValue.SetPrecision(8);
            olayer1.CreateField(oFieldValue, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            oFieldLongTime.SetWidth(20);
            olayer1.CreateField(oFieldLongTime, 1);

            //写入数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, polygon.id);
                oFeature.SetField(1, polygon.stormID);
                oFeature.SetField(3, "storm");

                oFeature.SetField(4, startTime.ToString());

                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                oFeature.SetField(5, minLog);
                oFeature.SetField(6, minLat);
                oFeature.SetField(7, maxLog);
                oFeature.SetField(8, maxLat);

                oFeature.SetField(9, polygon.area);
                oFeature.SetField(10, polygon.avgRainfall);
                oFeature.SetField(11, polygon.volume);
                oFeature.SetField(12, polygon.maxRainfall);
                oFeature.SetField(13, polygon.minRainfall);
                oFeature.SetField(14, polygon.power);
                DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
                long timeStamp = (long)(startTime - time1970).TotalSeconds; // 相差秒数
                oFeature.SetField(15, timeStamp.ToString());
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "POLYGON (";
                foreach (Line line in polygon.lines)
                {//写出多边形中每条线
                    polygonStr += "(";
                    foreach (Node node in line.nodes)
                    {
                        int _row = node.row;//点行号
                        int _col = node.col;//点列号

                        double log = startLog + (_col + 1) * resolution;//经度
                        double lat = startLat + (row - _row - 1) * resolution;//纬度

                        polygonStr += log.ToString() + " " + lat.ToString();
                        polygonStr += ",";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer1.Dispose();
            ds1.Dispose();
        }

        /// <summary>
        /// 基于空间图像的矢量提取
        /// </summary>
        /// <param name="oriPath"></param>
        /// <param name="valueScale"></param>
        /// <param name="timeCell"></param>
        /// <param name="idPath"></param>
        /// <param name="outPath"></param>
        public static void TifToShp(string oriPath, double valueScale, double timeCell, string spPath, string outPath)
        {
            //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

            //打开hdf文件
            Dataset oriDs = Gdal.Open(oriPath, Access.GA_ReadOnly);
            Dataset spDs = Gdal.Open(spPath, Access.GA_ReadOnly);
            int col = oriDs.RasterXSize;//列数
            int row = oriDs.RasterYSize;//行数
            Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
            Band spBand1 = spDs.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            oriDs.GetGeoTransform(argout);//读取地理坐标信息

            string[] metadatas = oriDs.GetMetadata("");//获取元数据
            double startLog = 70.0;//起始经度
            double startLat = 3.0;//起始维度
            double endLog = 140.0;//结束经度
            double endLat = 53.0;//结束维度
            double mScale = 1.0;//比例
            string dataType = "";
            string imgDate = "";
            string fillValue = "";
            double resolution = 0.1;//分辨率

            string fileName = Path.GetFileName(oriPath);
            DateTime startTime = DateTime.ParseExact(fileName.Substring(0, 6)+"01000000", "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);//起始时间 
            
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        mScale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "DataType":
                        dataType = mdArr[1];//起始经度
                        break;
                    case "ImageDate":
                        imgDate = mdArr[1];
                        break;
                    case "FillValue":
                        fillValue = mdArr[1];
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }

            double[] oriData = new double[row * col];//存储
            int[] spData = new int[row * col];//存储
            oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
            spBand1.ReadRaster(0, 0, col, row, spData, col, row, 0, 0);//读取数据
            oriDs.Dispose();
            spDs.Dispose();

            double[,] oriImg = new double[row, col];//二维数组，边缘不处理
            int[,] spImg = new int[row, col];//二维数组，边缘不处理
            for (int i = 0; i < spData.Length; i++)
            {//将一维数组转换为二维，方便处理
                int _rowNow = i / col;//行号
                int _colNow = i % col;//列号
                if (_rowNow == 0 || _colNow == 0 || _rowNow == spImg.GetLength(0) - 1 || _colNow == spImg.GetLength(1) - 1) continue;//边界点不添加
                if (oriData[i] > 0.0)
                {
                    oriImg[_rowNow, _colNow] = oriData[i] * valueScale * mScale;//乘以系数
                }
                else
                {//将0和负值赋值为0
                    oriImg[_rowNow, _colNow] = 0.0;
                }
                if (spData[i] > 0)
                {
                    spImg[_rowNow, _colNow] = spData[i];
                }
                else
                {
                    spImg[_rowNow, _colNow] = 0;
                }
            }
            
            #region 寻找结点
            List<Node> nodes = new List<Node>();//结点链表
            for (int i = 0; i < spImg.GetLength(0) - 1; i++)
            {//行循环
                for (int j = 0; j < spImg.GetLength(1) - 1; j++)
                {
                    int ltv = spImg[i, j];//左上角栅格值
                    int rtv = spImg[i, j + 1];//右上角栅格值
                    int lbv = spImg[i + 1, j];//左下角栅格值
                    int rbv = spImg[i + 1, j + 1];//右下角栅格值
                    if (ltv != rtv && rtv == lbv && lbv == rbv)
                    {//左上角不同，左和上方向
                        string dir1 = "l";//左方向
                        string dir2 = "t";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 1;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (ltv > 0)
                        {
                            node.power = ltv;
                        }
                        else
                        {
                            node.power = rtv;
                        }
                        nodes.Add(node);
                    }
                    else if (rtv != ltv && ltv == lbv && lbv == rbv)
                    {//右上角不同，上和右方向
                        string dir1 = "t";//左方向
                        string dir2 = "r";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 2;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rtv > 0)
                        {
                            node.power = rtv;
                        }
                        else
                        {
                            node.power = ltv;
                        }
                        nodes.Add(node);
                    }
                    else if (lbv != ltv && ltv == rtv && rtv == rbv)
                    {//左下角不同，下和左方向
                        string dir1 = "b";//左方向
                        string dir2 = "l";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 3;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (lbv > 0)
                        {
                            node.power = lbv;

                        }
                        else
                        {
                            node.power = ltv;
                        }
                        nodes.Add(node);
                    }
                    else if (rbv != ltv && ltv == rtv && rtv == lbv)
                    {//右下角不同，右和下方向
                        string dir1 = "r";//左方向
                        string dir2 = "b";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 4;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rbv > 0)
                        {
                            node.power = rbv;
                        }
                        else
                        {
                            node.power = ltv;
                        }
                        nodes.Add(node);
                    }
                    else if (ltv == rbv && ltv != rtv && rtv == lbv)
                    {//对角相等，相邻不等
                        string dir1 = "n";//左方向
                        string dir2 = "n";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        if (ltv > 0)
                        {//左上角和右下角值大于零
                            node.type = 5;
                            node.power = ltv;
                        }
                        else
                        {//右上角和左下角值大于零
                            node.type = 6;
                            node.power = rtv;
                        }
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                }
            }
            #endregion

            #region 所有线
            List<Line> lines = new List<Line>();
            //结点连成线
            for (int i = 0; i < nodes.Count; i++)
            {//寻找每一条线
                if (nodes[i].isUsed == true || nodes[i].type > 4) continue;//被使用，跳出本次循环
                Node headNode = nodes[i];//头结点
                headNode.isUsed = true;//记录被使用
                headNode.outDir = headNode.dir1;//记录出去的方向
                nodes[i] = headNode;//进行保存
                Line line = new Line();//新建一条线
                int minRow = headNode.row;
                int minCol = headNode.col;
                int maxRow = headNode.row;
                int maxCol = headNode.col;
                //line.nodes.Add(headNode);//
                List<Node> lineNodes = new List<Node>();//线中所有点
                lineNodes.Add(headNode);//将第一个点添加进去
                Node tailNode = headNode;//尾结点
                //Node nextNode = new Node();//用来记录找到的下一个结点
                double lineLength = 0.0;//线的长度
                do
                {
                    int startNodeRow = tailNode.row;
                    int startNodeCol = tailNode.col;
                    tailNode = GetNextNode(tailNode, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);

                    //计算周长
                    double logInterval = Math.Abs(tailNode.col - startNodeCol) * resolution;//经度差值
                    if(logInterval==0.0)
                    {//竖线
                        double latInterval= Math.Abs(tailNode.row - startNodeRow) * resolution;//纬度差值
                        double _length = 20037.0 * latInterval / 180.0;//经线长度为20017km（百度百科），每条经线长度都相同
                        lineLength += _length;
                    }
                    else
                    {//横线
                        const double equatorLength= 40076.0;//赤道周长，单位km
                        double lineLat = startLat + (row - tailNode.row) * resolution;//纬度
                        double equatorLengthNow = equatorLength * Math.Cos(lineLat*Math.PI/180.0);//当前纬线周长
                        double _length = equatorLengthNow * logInterval / 360.0;
                        lineLength += _length;
                    }

                    lineNodes.Add(tailNode);//将点添加进去
                } while (tailNode.id != headNode.id);

                //判断是内环还是外环
                Node firstNode = lineNodes[0];//第一个结点
                if(firstNode.type==1)
                {
                    if (spImg[firstNode.row, firstNode.col] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 2)
                {
                    if (spImg[firstNode.row, firstNode.col + 1] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 3)
                {
                    if (spImg[firstNode.row + 1, firstNode.col] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 4)
                {
                    if (spImg[firstNode.row + 1, firstNode.col + 1] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                
                line.id = lines.Count;
                line.nodes = lineNodes;//进行保存
                line.minRow = minRow;
                line.minCol = minCol;
                line.maxRow = maxRow;
                line.maxCol = maxCol;
                line.power = headNode.power;//值
                line.length = lineLength;//线的长度
                lines.Add(line);
            }
            #endregion

            #region 线构成面
            List<Polygon> polygons = new List<Polygon>();//所有面
            for (int i = 0; i < lines.Count; i++)
            {//循环每一条线
                if (lines[i].type != 0) continue;//不是外环，退出本次循环
                Polygon polygon = new Polygon();
                List<Line> pLines = new List<Line>();
                pLines.Add(lines[i]);//添加外环
                polygon.id = polygons.Count;
                polygon.minCol = lines[i].minCol;
                polygon.minRow = lines[i].minRow;
                polygon.maxCol = lines[i].maxCol;
                polygon.maxRow = lines[i].maxRow;
                polygon.power = lines[i].power;
                for (int j = i + 1; j < lines.Count; j++)
                {//寻找该外环包含的内环，内环肯定在后面
                    //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
                    if (lines[j].type == 0) continue;//外环
                    if (lines[j].minRow > lines[i].minRow && lines[j].minCol > lines[i].minCol && lines[j].maxRow < lines[i].maxRow && lines[j].maxCol < lines[i].maxCol)
                    {//邻接矩形包含，进一步判断
                        //只要一个点在外环内，就是内环
                        bool isIn = IsInPolygonNew(lines[j].nodes[0], lines[i].nodes);//判断是否在外环里面
                        if (isIn)
                        {//内环
                            Line line = lines[j];//取出线
                            line.nodes.Reverse();//顺序反转
                            //line.type = 1;
                            lines[j] = line;//保存修改后的
                            pLines.Add(line);//添加内环
                        }
                    }
                }
                polygon.lines = pLines;
                double avgRainfall = 0.0;//平均降雨量
                double volume = 0.0;//累计降雨量
                double maxRainfall = 0.0;//最大降雨量
                double minRainfall = double.MaxValue;//最小降雨量
                double area = 0.0;//面积
                double _rowCore = 0.0;//重心行号中间量
                double _colCore = 0.0;//重心列号中间量
                for (int _row = polygon.minRow + 1; _row <= polygon.maxRow; _row++)
                {//行循环，行列号对应节点左上角栅格
                    double rasterStartLat = startLat + (row - _row - 1) * resolution;//栅格下边缘纬度
                    double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
                    double rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积
                    for (int _col = polygon.minCol + 1; _col <= polygon.maxCol; _col++)
                    {//列循环
                        //if (idImg[_row, _col] == polygon.stormID)
                        if(IsInPolygonNew(_row-0.5f, _col-0.5f, pLines[0].nodes))
                        {//当前暴雨对象
                            float _rowF = _row - 0.5f;//栅格行列号转为矢量行列号
                            float _colF = _col - 0.5f;
                            if (IsInPolygonNew(_rowF, _colF, polygon.lines[0].nodes))
                            {//包含关系
                                area += rasterArea;//增加面积
                                //double rainfall = oriImg[_row, _col] / timeCell;//换算为每小时
                                double rainfall = oriImg[_row, _col];//每半小时
                                if (rainfall < 0.0) rainfall = 0;
                                if (rainfall > maxRainfall) maxRainfall = rainfall;//最大降雨量
                                if (rainfall < minRainfall) minRainfall = rainfall;
                                double _volume = rasterArea * rainfall;//暴雨总降雨量累加
                                volume += _volume;
                                //加权
                                _rowCore += _volume * _row;
                                _colCore += _volume * _col;
                            }
                        }
                    }
                }
                //平均
                polygon.coreRow= _rowCore/ volume;
                polygon.coreCol = _colCore / volume;

                avgRainfall = volume / area;//计算平均降雨量
                polygon.avgRainfall = avgRainfall;//保存平均降雨量
                polygon.maxRainfall = maxRainfall;//保存最大降雨量
                polygon.minRainfall = minRainfall;//保存最大降雨量
                polygon.area = area;

                volume = volume * 1000;//立方米
                polygon.volume = volume;
                polygon.length = polygon.lines[0].length;//周长
                polygon.minRec= GetMinAreaRec(polygon);
                polygon.minOutCir = GetMinOutCir(polygon);
                polygon.maxInCir = GetMaxInCir(polygon, 0.5);
                polygons.Add(polygon);
            }
            #endregion

            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = outPath;
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);

            #region 接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("StormID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStateID = new FieldDefn("StateID", FieldType.OFTString);
            oFieldStateID.SetWidth(20);
            olayer1.CreateField(oFieldStateID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
            oFieldName.SetWidth(20);
            olayer1.CreateField(oFieldName, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldTime.SetWidth(20);
            olayer1.CreateField(oFieldTime, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            oFieldLongTime.SetWidth(20);
            olayer1.CreateField(oFieldLongTime, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
            oFieldMinLog.SetWidth(20);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建area字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            olayer1.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgRain", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            //创建体积字段
            FieldDefn oFieldVolume = new FieldDefn("Volume", FieldType.OFTReal);
            oFieldVolume.SetWidth(20);
            oFieldVolume.SetPrecision(8);
            olayer1.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("MaxRain", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMaxRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("MinRain", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMinRainFall, 1);

            //创建Power字段
            FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTInteger);
            olayer1.CreateField(oFieldPower, 1);

            //创建周长字段
            FieldDefn oFieldLength = new FieldDefn("Length", FieldType.OFTReal);
            oFieldLength.SetWidth(20);
            oFieldLength.SetPrecision(8);
            olayer1.CreateField(oFieldLength, 1);

            //创建质心字段
            FieldDefn oFieldLogCore = new FieldDefn("CoreLog", FieldType.OFTReal);
            oFieldLogCore.SetWidth(20);
            oFieldLogCore.SetPrecision(8);
            olayer1.CreateField(oFieldLogCore, 1);

            //创建质心字段
            FieldDefn oFieldLatCore = new FieldDefn("CoreLat", FieldType.OFTReal);
            oFieldLatCore.SetWidth(20);
            oFieldLatCore.SetPrecision(8);
            olayer1.CreateField(oFieldLatCore, 1);

            //创建形状系数字段
            FieldDefn oFieldSI = new FieldDefn("SI", FieldType.OFTReal);
            oFieldSI.SetWidth(20);
            oFieldSI.SetPrecision(8);
            olayer1.CreateField(oFieldSI, 1);

            //创建最大长度字段
            FieldDefn oFieldLMax = new FieldDefn("LMax", FieldType.OFTReal);
            oFieldLMax.SetWidth(20);
            oFieldLMax.SetPrecision(8);
            olayer1.CreateField(oFieldLMax, 1);

            //创建最大宽度字段
            FieldDefn oFieldWMax = new FieldDefn("WMax", FieldType.OFTReal);
            oFieldWMax.SetWidth(20);
            oFieldWMax.SetPrecision(8);
            olayer1.CreateField(oFieldWMax, 1);

            //创建偏心率字段
            FieldDefn oFieldERatio = new FieldDefn("ERatio", FieldType.OFTReal);
            oFieldERatio.SetWidth(20);
            oFieldERatio.SetPrecision(8);
            olayer1.CreateField(oFieldERatio, 1);

            //创建矩形度字段
            FieldDefn oFieldRecDeg = new FieldDefn("RecDeg", FieldType.OFTReal);
            oFieldRecDeg.SetWidth(20);
            oFieldRecDeg.SetPrecision(8);
            olayer1.CreateField(oFieldRecDeg, 1);

            //创建圆形度字段
            FieldDefn oFieldSphDeg = new FieldDefn("SphDeg", FieldType.OFTReal);
            oFieldSphDeg.SetWidth(20);
            oFieldSphDeg.SetPrecision(8);
            olayer1.CreateField(oFieldSphDeg, 1);

            //创建最小外接矩形点1
            FieldDefn oFieldMinRecP1X = new FieldDefn("RecP1X", FieldType.OFTString);
            oFieldMinRecP1X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP1X, 1);

            //创建最小外接矩形点1
            FieldDefn oFieldMinRecP1Y = new FieldDefn("RecP1Y", FieldType.OFTString);
            oFieldMinRecP1Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP1Y, 1);

            //创建最小外接矩形点2
            FieldDefn oFieldMinRecP2X = new FieldDefn("RecP2X", FieldType.OFTString);
            oFieldMinRecP2X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP2X, 1);

            //创建最小外接矩形点2
            FieldDefn oFieldMinRecP2Y = new FieldDefn("RecP2Y", FieldType.OFTString);
            oFieldMinRecP2Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP2Y, 1);

            //创建最小外接矩形点3
            FieldDefn oFieldMinRecP3X = new FieldDefn("RecP3X", FieldType.OFTString);
            oFieldMinRecP3X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP3X, 1);

            //创建最小外接矩形点3
            FieldDefn oFieldMinRecP3Y = new FieldDefn("RecP3Y", FieldType.OFTString);
            oFieldMinRecP3Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP3Y, 1);

            //创建最小外接矩形点4
            FieldDefn oFieldMinRecP4X = new FieldDefn("RecP4X", FieldType.OFTString);
            oFieldMinRecP4X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP4X, 1);

            //创建最小外接矩形点4
            FieldDefn oFieldMinRecP4Y = new FieldDefn("RecP4Y", FieldType.OFTString);
            oFieldMinRecP4Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP4Y, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP1 = new FieldDefn("P1", FieldType.OFTReal);
            //oFieldP1.SetWidth(10);
            //oFieldP1.SetPrecision(8);
            //olayer1.CreateField(oFieldP1, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP2 = new FieldDefn("P2", FieldType.OFTReal);
            //oFieldP2.SetWidth(10);
            //oFieldP2.SetPrecision(8);
            //olayer1.CreateField(oFieldP2, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP3 = new FieldDefn("P3", FieldType.OFTReal);
            //oFieldP3.SetWidth(10);
            //oFieldP3.SetPrecision(8);
            //olayer1.CreateField(oFieldP3, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP4 = new FieldDefn("P4", FieldType.OFTReal);
            //oFieldP4.SetWidth(10);
            //oFieldP4.SetPrecision(8);
            //olayer1.CreateField(oFieldP4, 1);
            #endregion

            //写入数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, polygon.id);
                //oFeature.SetField(1, polygon.stormID);
                oFeature.SetField(3, "storm");

                oFeature.SetField(4, startTime.ToString());
                DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
                long timeStamp = (long)(startTime - time1970).TotalSeconds; // 相差秒数
                oFeature.SetField(5, timeStamp.ToString());

                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //计算经纬度
                double CoreLog = startLog + (polygon.coreCol + 0.5) * resolution;//质心经度
                double CoreLat = startLat + (row - polygon.coreRow - 0.5) * resolution;//质心纬度

                //	形状系数（SI）：面积（A）/周长（P）
                double si = (4 * Math.Sqrt(polygon.area)) / polygon.length;
                double eRatio = polygon.minRec.width / polygon.minRec.length;
                double recDeg = polygon.area / ((polygon.minRec.length * polygon.minRec.width)*123.93*Math.Cos((maxLat+minLat)/2*Math.PI/180));//最小外包矩形面积为近似计算
                double sphDeg = polygon.maxInCir.r / polygon.minOutCir.r;

                oFeature.SetField(6, minLog);
                oFeature.SetField(7, minLat);
                oFeature.SetField(8, maxLog);
                oFeature.SetField(9, maxLat);

                oFeature.SetField(10, polygon.area);
                oFeature.SetField(11, polygon.avgRainfall);
                oFeature.SetField(12, polygon.volume);
                oFeature.SetField(13, polygon.maxRainfall);
                oFeature.SetField(14, polygon.minRainfall);
                oFeature.SetField(15, polygon.power);
                oFeature.SetField(16, polygon.length);
                oFeature.SetField(17, CoreLog);
                oFeature.SetField(18, CoreLat);
                oFeature.SetField(19, si);
                oFeature.SetField(20, polygon.minRec.length *resolution);
                oFeature.SetField(21, polygon.minRec.width * resolution);
                oFeature.SetField(22, eRatio);
                oFeature.SetField(23, recDeg);
                oFeature.SetField(24, sphDeg);

                double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
                double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
                double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
                double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

                oFeature.SetField(25, p1t[0]);
                oFeature.SetField(26, p1t[1]);
                oFeature.SetField(27, p2t[0]);
                oFeature.SetField(28, p2t[1]);
                oFeature.SetField(29, p3t[0]);
                oFeature.SetField(30, p3t[1]);
                oFeature.SetField(31, p4t[0]);
                oFeature.SetField(32, p4t[1]);

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "POLYGON (";
                foreach (Line line in polygon.lines)
                {//写出多边形中每条线
                    polygonStr += "(";
                    foreach (Node node in line.nodes)
                    {
                        int _row = node.row;//点行号
                        int _col = node.col;//点列号

                        double log = startLog + (_col + 1) * resolution;//经度
                        double lat = startLat + (row - _row - 1) * resolution;//纬度

                        polygonStr += log.ToString() + " " + lat.ToString();
                        polygonStr += ",";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer1.Dispose();
            ds1.Dispose();

            #region 最小面积外包矩形输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile2 = Path.GetDirectoryName(outPath)+"\\1\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinAreaRec.shp";
            Ogr.RegisterAll();

            string strDriver2 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
            if (oDriver2 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
            if (ds2 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
            Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn2 = olayer2.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                Feature oFeature = new Feature(oDefn2);
                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
                double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
                double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
                double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

                string polygonStr = "POLYGON (("+ p1t[0] + " " + p1t[1] + "," + p2t[0] + " " + p2t[1] + "," + p4t[0] + " " + p4t[1] + "," + p3t[0] + " " + p3t[1] + "," + p1t[0] + " " + p1t[1]+"))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer2.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer2.Dispose();
            ds2.Dispose();
            */
            #endregion

            #region 最小面积外接圆输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile2 = Path.GetDirectoryName(outPath)+"\\1\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinOutCir.shp";
            Ogr.RegisterAll();

            string strDriver2 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
            if (oDriver2 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
            if (ds2 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
            Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn2 = olayer2.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                Feature oFeature = new Feature(oDefn2);
                //计算经纬度
                //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

                string polygonStr = "POLYGON ((";
                double cx = polygon.minOutCir.x;
                double cy = polygon.minOutCir.y;
                double cr = polygon.minOutCir.r;
                for (int i=0;i<=360;i++)
                {
                    double de = i * Math.PI / 180;//弧度制
                    double x = cx + cr*Math.Sin(de);
                    double y = cy + cr * Math.Cos(de);

                    double log= startLog + (y + 1) * resolution;//经度
                    double lat = startLat + (row - x - 1) * resolution;//纬度

                    polygonStr += log + " " + lat + ",";
                }


                polygonStr=polygonStr.TrimEnd(',');
                polygonStr+= "))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer2.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer2.Dispose();
            ds2.Dispose();
            */
            #endregion

            #region 最大面积内切圆输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile2 = Path.GetDirectoryName(outPath) + "\\1\\" + Path.GetFileNameWithoutExtension(outPath) + "_MaxInCir.shp";
            Ogr.RegisterAll();

            string strDriver2 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
            if (oDriver2 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
            if (ds2 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
            Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn2 = olayer2.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                Feature oFeature = new Feature(oDefn2);
                //计算经纬度
                //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

                string polygonStr = "POLYGON ((";
                double cx = polygon.maxInCir.x;
                double cy = polygon.maxInCir.y;
                double cr = polygon.maxInCir.r;
                for (int i = 0; i <= 360; i++)
                {
                    double de = i * Math.PI / 180;//弧度制
                    double x = cx + cr * Math.Sin(de);
                    double y = cy + cr * Math.Cos(de);

                    double log = startLog + (y + 1) * resolution;//经度
                    double lat = startLat + (row - x - 1) * resolution;//纬度

                    polygonStr += log + " " + lat + ",";
                }


                polygonStr = polygonStr.TrimEnd(',');
                polygonStr += "))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer2.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer2.Dispose();
            ds2.Dispose();
            */
            #endregion
        }



        /// <summary>
        /// 海洋热浪图像的矢量提取
        /// </summary>
        /// <param name="oriFilePath"></param>
        /// <param name="valueScale"></param>
        /// <param name="timeCell"></param>
        /// <param name="idPath"></param>
        /// <param name="outPath"></param>
        //public static void MarineHeatwavesTifToShp(string timeFilePath, string uFilePath, string vFilePath, string outPath, double timeCell)
        //{
        //    DateTime time1 = DateTime.Now;
        //    //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
        //    //Gdal.AllRegister();//注册所有的格式驱动
        //    //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

        //    //打开hdf文件
        //    //Dataset oriDs = Gdal.Open(oriFilePath, Access.GA_ReadOnly);
        //    Dataset timeDs = Gdal.Open(timeFilePath, Access.GA_ReadOnly);
        //    Dataset uDs = Gdal.Open(uFilePath, Access.GA_ReadOnly);
        //    Dataset vDs = Gdal.Open(vFilePath, Access.GA_ReadOnly);
        //    int col = timeDs.RasterXSize;//列数
        //    int row = timeDs.RasterYSize;//行数
        //    //Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
        //    Band timeBand1 = timeDs.GetRasterBand(1);//读取波段
        //    Band uBand1 = uDs.GetRasterBand(1);//读取波段
        //    Band vBand1 = vDs.GetRasterBand(1);//读取波段

        //    double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
        //    timeDs.GetGeoTransform(argout);//读取地理坐标信息

        //    string[] metadatas = timeDs.GetMetadata("");//获取元数据
        //    double startLog = 0.0;//起始经度
        //    double startLat = 0.0;//起始维度
        //    double endLog = 0.0;//结束经度
        //    double endLat = 0.0;//结束维度
        //    double scale = 0.0;//比例
        //    int fillValue = 0;
        //    double resolution = 0.0;//分辨率

        //    //string fileName = Path.GetFileNameWithoutExtension(timeFilePath);
        //    //DateTime startTime = DateTime.ParseExact(fileName, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);//起始时间 
        //    string fileName = Path.GetFileNameWithoutExtension(timeFilePath).Substring(0, 6);
        //    DateTime startTime = DateTime.ParseExact(fileName, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);//起始时间 
        //    //DateTime startTime = DateTime.Now;//起始时间 

        //    foreach (string md in metadatas)
        //    {//获取信息
        //        string[] mdArr = md.Split('=');
        //        switch (mdArr[0])
        //        {
        //            case "StartLog":
        //                startLog = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            case "StartLat":
        //                startLat = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            case "EndLog":
        //                endLog = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            case "EndLat":
        //                endLat = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            case "Scale":
        //                scale = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            case "FillValue":
        //                fillValue =Convert.ToInt32(mdArr[1]);
        //                break;
        //            case "Resolution":
        //                resolution = Convert.ToDouble(mdArr[1]);
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    string[] uMetadatas = uDs.GetMetadata("");//获取元数据
        //    double uSacle = 0.0;
        //    foreach (string md in metadatas)
        //    {//获取信息
        //        string[] mdArr = md.Split('=');
        //        switch (mdArr[0])
        //        {
        //            case "Scale":
        //                uSacle = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    string[] vMetadatas = vDs.GetMetadata("");//获取元数据
        //    double vSacle = 0.0;
        //    foreach (string md in metadatas)
        //    {//获取信息
        //        string[] mdArr = md.Split('=');
        //        switch (mdArr[0])
        //        {
        //            case "Scale":
        //                vSacle = Convert.ToDouble(mdArr[1]);//起始经度
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    //double[] oriData = new double[row * col];//存储
        //    int[] timeData = new int[row * col];//存储
        //    int[] uData = new int[row * col];//存储
        //    int[] vData = new int[row * col];//存储
        //    //oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
        //    timeBand1.ReadRaster(0, 0, col, row, timeData, col, row, 0, 0);//读取数据
        //    uBand1.ReadRaster(0, 0, col, row, uData, col, row, 0, 0);//读取数据
        //    vBand1.ReadRaster(0, 0, col, row, vData, col, row, 0, 0);//读取数据
        //    //oriDs.Dispose();
        //    timeBand1.Dispose();
        //    uBand1.Dispose();
        //    vBand1.Dispose();
        //    timeDs.Dispose();
        //    uDs.Dispose();
        //    vDs.Dispose();

        //    //double[,] oriImg = new double[row, col];//二维数组，边缘不处理
        //    int[,] timeImg = new int[row, col];//二维数组，边缘不处理
        //    int[,] uImg = new int[row, col];//二维数组，边缘不处理
        //    int[,] vImg = new int[row, col];//二维数组，边缘不处理
        //    for (int i = 0; i < timeData.Length; i++)
        //    {//将一维数组转换为二维，方便处理
        //        int _rowNow = i / col;//行号
        //        int _colNow = i % col;//列号
        //        if (_rowNow == 0 || _colNow == 0 || _rowNow == timeImg.GetLength(0) - 1 || _colNow == timeImg.GetLength(1) - 1) continue;//边界点不添加

        //        //oriImg[_rowNow, _colNow] = oriData[i] * scale;//乘以系数
        //        //timeImg[_rowNow, _colNow] = timeData[i];
        //        //uImg[_rowNow, _colNow] = uData[i];
        //        //vImg[_rowNow, _colNow] = vData[i];
        //        if (timeData[i] > 0)
        //        {
        //            timeImg[_rowNow, _colNow] = timeData[i];
        //        }
        //        if (uData[i] != -9999) uImg[_rowNow, _colNow] = uData[i];
        //        if (vData[i] != -9999) vImg[_rowNow, _colNow] = vData[i];
        //    }

        //    #region 寻找结点
        //    List<Node> nodes = new List<Node>();//结点链表
        //    for (int i = 0; i < timeImg.GetLength(0) - 1; i++)
        //    {//行循环
        //        for (int j = 0; j < timeImg.GetLength(1) - 1; j++)
        //        {
        //            int ltv = timeImg[i, j];//左上角栅格值
        //            int rtv = timeImg[i, j + 1];//右上角栅格值
        //            int lbv = timeImg[i + 1, j];//左下角栅格值
        //            int rbv = timeImg[i + 1, j + 1];//右下角栅格值
        //            if ((ltv == 0 && rtv > 0 && lbv > 0 && rbv > 0)||(ltv > 0 && rtv == 0 && lbv == 0&& rbv == 0))
        //            {//左上角不同，左和上方向
        //                string dir1 = "l";//左方向
        //                string dir2 = "t";//上方向
        //                //double log = startLog + j * resolution;//经度
        //                //double lat = endLat - i * resolution;//纬度
        //                Node node = new Node();
        //                node.id = nodes.Count;
        //                node.type = 1;
        //                node.row = i;
        //                node.col = j;
        //                node.dir1 = dir1;
        //                node.dir2 = dir2;
        //                node.isUsed = false;
        //                nodes.Add(node);
        //            }
        //            else if ((rtv == 0 && ltv > 0 && lbv > 0 && rbv > 0)|| (rtv > 0 && ltv == 0 && lbv == 0 && rbv == 0))
        //            {//右上角不同，上和右方向
        //                string dir1 = "t";//左方向
        //                string dir2 = "r";//上方向
        //                //double log = startLog + j * resolution;//经度
        //                //double lat = endLat - i * resolution;//纬度
        //                Node node = new Node();
        //                node.id = nodes.Count;
        //                node.type = 2;
        //                node.row = i;
        //                node.col = j;
        //                node.dir1 = dir1;
        //                node.dir2 = dir2;
        //                node.isUsed = false;
        //                nodes.Add(node);
        //            }
        //            else if ((lbv == 0 && ltv > 0 && rtv > 0 && rbv > 0)||(lbv > 0 && ltv == 0 && rtv == 0 && rbv == 0))
        //            {//左下角不同，下和左方向
        //                string dir1 = "b";//左方向
        //                string dir2 = "l";//上方向
        //                //double log = startLog + j * resolution;//经度
        //                //double lat = endLat - i * resolution;//纬度
        //                Node node = new Node();
        //                node.id = nodes.Count;
        //                node.type = 3;
        //                node.row = i;
        //                node.col = j;
        //                node.dir1 = dir1;
        //                node.dir2 = dir2;
        //                node.isUsed = false;
        //                nodes.Add(node);
        //            }
        //            else if ((rbv == 0 && ltv > 0 && rtv > 0 && lbv >0)|| (rbv > 0 && ltv == 0 && rtv == 0 && lbv == 0))
        //            {//右下角不同，右和下方向
        //                string dir1 = "r";//左方向
        //                string dir2 = "b";//上方向
        //                //double log = startLog + j * resolution;//经度
        //                //double lat = endLat - i * resolution;//纬度
        //                Node node = new Node();
        //                node.id = nodes.Count;
        //                node.type = 4;
        //                node.row = i;
        //                node.col = j;
        //                node.dir1 = dir1;
        //                node.dir2 = dir2;
        //                node.isUsed = false;
        //                nodes.Add(node);
        //            }
        //            else if ((ltv == 0 && rbv ==0 && rtv > 0 && lbv > 0)||(ltv > 0 && rbv > 0 && rtv == 0 && lbv == 0))
        //            {//对角相等，相邻不等
        //                string dir1 = "n";//左方向
        //                string dir2 = "n";//上方向
        //                //double log = startLog + j * resolution;//经度
        //                //double lat = endLat - i * resolution;//纬度
        //                Node node = new Node();
        //                node.id = nodes.Count;
        //                if (ltv > 0)
        //                {//左上角和右下角值大于零
        //                    node.type = 5;
        //                    node.power = ltv;
        //                    node.stormID = timeImg[i, j];//左上角栅格值
        //                }
        //                else
        //                {//右上角和左下角值大于零
        //                    node.type = 6;
        //                    node.power = rtv;
        //                    node.stormID = timeImg[i, j + 1];//右上角栅格值
        //                }
        //                node.row = i;
        //                node.col = j;
        //                node.dir1 = dir1;
        //                node.dir2 = dir2;
        //                node.isUsed = false;
        //                nodes.Add(node);
        //            }
        //        }
        //    }
        //    Node[] nodeArr = nodes.ToArray();
        //    #endregion

        //    #region 所有线
        //    List<Line> lines = new List<Line>();//
        //    //结点连成线
        //    for (int i = 0; i < nodeArr.Length; i++)
        //    {//寻找每一条线
        //        if (nodeArr[i].isUsed == true || nodeArr[i].type > 4) continue;//被使用，跳出本次循环
        //        Node headNode = nodeArr[i];//头结点
        //        headNode.isUsed = true;//记录被使用
        //        headNode.outDir = headNode.dir1;//记录出去的方向
        //        nodeArr[i] = headNode;//进行保存
        //        Line line = new Line();//新建一条线
        //        int minRow = headNode.row;
        //        int minCol = headNode.col;
        //        int maxRow = headNode.row;
        //        int maxCol = headNode.col;
        //        //line.nodes.Add(headNode);//
        //        List<Node> lineNodes = new List<Node>();//线中所有点
        //        lineNodes.Add(headNode);//将第一个点添加进去
        //        Node tailNode = headNode;//尾结点
        //        //Node nextNode = new Node();//用来记录找到的下一个结点
        //        double lineLength = 0.0;//线的长度
        //        do
        //        {
        //            int startNodeRow = tailNode.row;
        //            int startNodeCol = tailNode.col;
        //            tailNode = GetNextNode(tailNode, ref nodeArr, ref minRow, ref minCol, ref maxRow, ref maxCol);

        //            //计算周长
        //            double logInterval = Math.Abs(tailNode.col - startNodeCol) * resolution;//经度差值
        //            if (logInterval == 0.0)
        //            {//竖线
        //                double latInterval = Math.Abs(tailNode.row - startNodeRow) * resolution;//纬度差值
        //                double _length = Earth.ML*0.001 * latInterval / 180.0;//经线长度为20017km（百度百科），每条经线长度都相同
        //                lineLength += _length;
        //            }
        //            else
        //            {//横线
        //                double lineLat = startLat + (row - tailNode.row) * resolution;//纬度
        //                double equatorLengthNow = Earth.EC*0.001 * Math.Cos(lineLat * Math.PI / 180.0);//当前纬线周长
        //                double _length = equatorLengthNow * logInterval / 360.0;
        //                lineLength += _length;
        //            }

        //            lineNodes.Add(tailNode);//将点添加进去
        //        } while (tailNode.id != headNode.id);

        //        //判断是内环还是外环
        //        Node firstNode = lineNodes[0];//第一个结点
        //        if (firstNode.type == 1)
        //        {
        //            if (timeImg[firstNode.row, firstNode.col] > 0)//右下角栅格值
        //            {
        //                line.type = 0;//外环
        //            }
        //            else
        //            {
        //                line.type = 1;//内环
        //            }
        //        }
        //        else if (firstNode.type == 2)
        //        {
        //            if (timeImg[firstNode.row, firstNode.col + 1] > 0)//右下角栅格值
        //            {
        //                line.type = 0;//外环
        //            }
        //            else
        //            {
        //                line.type = 1;//内环
        //            }
        //        }
        //        else if (firstNode.type == 3)
        //        {
        //            if (timeImg[firstNode.row + 1, firstNode.col] > 0)//右下角栅格值
        //            {
        //                line.type = 0;//外环
        //            }
        //            else
        //            {
        //                line.type = 1;//内环
        //            }
        //        }
        //        else if (firstNode.type == 4)
        //        {
        //            if (timeImg[firstNode.row + 1, firstNode.col + 1] > 0)//右下角栅格值
        //            {
        //                line.type = 0;//外环
        //            }
        //            else
        //            {
        //                line.type = 1;//内环
        //            }
        //        }

        //        line.id = lines.Count;
        //        line.nodes = lineNodes;//进行保存
        //        line.minRow = minRow;
        //        line.minCol = minCol;
        //        line.maxRow = maxRow;
        //        line.maxCol = maxCol;
        //        line.power = headNode.power;//值
        //        line.length = lineLength;//线的长度
        //        lines.Add(line);
        //    }
        //    Line[] lineArr = lines.ToArray();
        //    #endregion

        //    #region 线构成面
        //    List<Polygon> polygons = new List<Polygon>();//所有面
        //    for (int i = 0; i < lineArr.Length; i++)
        //    {//循环每一条线
        //        if (lineArr[i].type != 0) continue;//不是外环，退出本次循环
        //        Polygon polygon = new Polygon();
        //        List<Line> pLines = new List<Line>();
        //        pLines.Add(lineArr[i]);//添加外环
        //        polygon.id = polygons.Count;
        //        polygon.minCol = lineArr[i].minCol;
        //        polygon.minRow = lineArr[i].minRow;
        //        polygon.maxCol = lineArr[i].maxCol;
        //        polygon.maxRow = lineArr[i].maxRow;
        //        polygon.power = lineArr[i].power;
        //        for (int j = i + 1; j < lineArr.Length; j++)
        //        {//寻找该外环包含的内环，内环肯定在后面
        //            //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
        //            if (lineArr[j].type == 0) continue;//外环
        //            if (lineArr[j].minRow > lineArr[i].minRow && lineArr[j].minCol > lineArr[i].minCol && lineArr[j].maxRow < lineArr[i].maxRow && lineArr[j].maxCol < lineArr[i].maxCol)
        //            {//邻接矩形包含，进一步判断
        //                //只要一个点在外环内，就是内环
        //                bool isIn = IsInPolygonNew(lineArr[j].nodes[0], lineArr[i].nodes);//判断是否在外环里面
        //                if (isIn)
        //                {//内环
        //                    Line line = lineArr[j];//取出线
        //                    line.nodes.Reverse();//顺序反转
        //                    //line.type = 1;
        //                    lineArr[j] = line;//保存修改后的
        //                    pLines.Add(line);//添加内环
        //                }
        //            }
        //        }
        //        polygon.lines = pLines;
        //        double volume = 0.0;//温度异常累加
        //        double intensityMean = 0.0;//平均温度异常
        //        double intensityMax = 0.0;//最大温度异常
        //        double intensityMin = double.MaxValue;//最小温度异常
        //        double uArea = 0.0;//经线方向速度与面积乘积
        //        double vArea = 0.0;//纬线方向速度与面积成绩
        //        //double avgRainfall = 0.0;//平均降雨量
        //        //double volume = 0.0;//累计降雨量
        //        //double maxRainfall = 0.0;//最大降雨量
        //        //double minRainfall = double.MaxValue;//最小降雨量
        //        double area = 0.0;//面积
        //        double _rowCore = 0.0;//重心行号中间量
        //        double _colCore = 0.0;//重心列号中间量
        //        for (int _row = polygon.minRow + 1; _row <= polygon.maxRow; _row++)
        //        {//行循环，行列号对应节点左上角栅格
        //            double rasterStartLat = startLat + (row - _row - 1) * resolution;//栅格下边缘纬度
        //            double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
        //            double rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积
        //            for (int _col = polygon.minCol + 1; _col <= polygon.maxCol; _col++)
        //            {//列循环
        //                //if (idImg[_row, _col] == polygon.stormID)
        //                if (IsInPolygonNew(_row - 0.5f, _col - 0.5f, pLines[0].nodes))
        //                {//当前暴雨对象
        //                    float _rowF = _row - 0.5f;//栅格行列号转为矢量行列号
        //                    float _colF = _col - 0.5f;
        //                    if (IsInPolygonNew(_rowF, _colF, polygon.lines[0].nodes))
        //                    {//包含关系
        //                        double uSpeed = uImg[_row, _col];
        //                        double vSpeed = uImg[_row, _col];
        //                        uArea += uSpeed * rasterArea;
        //                        uArea += vSpeed * rasterArea;
        //                        area += rasterArea;//增加面积
        //                        //double rainfall = oriImg[_row, _col] / timeCell;//换算为每小时
        //                        double intensity = timeImg[_row, _col];//每半小时
        //                        if (intensity > intensityMax) intensityMax = intensity;//最大降雨量
        //                        if (intensity < intensityMin) intensityMin = intensity;
        //                        double _volume = rasterArea * intensity;//暴雨总降雨量累加
        //                        volume += _volume;
        //                        //加权
        //                        _rowCore += _volume * _row;
        //                        _colCore += _volume * _col;
        //                    }
        //                }
        //            }
        //        }
        //        //平均
        //        polygon.coreRow = _rowCore / volume;
        //        polygon.coreCol = _colCore / volume;

        //        intensityMean = volume / area;//计算平均降雨量
        //        polygon.intensityMean = intensityMean * scale;//保存平均降雨量
        //        polygon.intensityMax = intensityMax * scale;//保存最大降雨量
        //        polygon.intensityMin = intensityMin * scale;//保存最大降雨量
        //        polygon.area = area;

        //        polygon.uSpeed = uArea / area;
        //        polygon.uSpeed = vArea / area;

        //        //volume = volume * 1000;//立方米
        //        //polygon.volume = volume;
        //        polygon.length = polygon.lines[0].length;//周长
        //        polygon.minRec = GetMinAreaRec(polygon);
        //        polygon.minOutCir = GetMinOutCir(polygon);
        //        polygon.maxInCir = GetMaxInCir(polygon, 0.5);
        //        polygons.Add(polygon);
        //    }
        //    #endregion

        //    //保存shp
        //    Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
        //    // 为了使属性表字段支持中文，请添加下面这句
        //    Gdal.SetConfigOption("SHAPE_ENCODING", "");
        //    string strVectorFile1 = outPath;
        //    Ogr.RegisterAll();

        //    string strDriver = "ESRI Shapefile";
        //    OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
        //    if (oDriver == null)
        //    {
        //        //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
        //        return;
        //    }
        //    DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
        //    if (ds1 == null)
        //    {
        //        //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
        //        return;
        //    }

        //    string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
        //    OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
        //    Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);

        //    #region 接下来创建属性表字段
        //    // 先创建一个叫FieldID的整型属性
        //    FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
        //    olayer1.CreateField(oFieldID, 1);

        //    // 先创建一个叫FieldID的整型属性
        //    FieldDefn oFieldStormID = new FieldDefn("EventID", FieldType.OFTInteger);
        //    olayer1.CreateField(oFieldStormID, 1);

        //    // 先创建一个叫FieldID的整型属性
        //    FieldDefn oFieldStateID = new FieldDefn("StateID", FieldType.OFTString);
        //    oFieldStateID.SetWidth(20);
        //    olayer1.CreateField(oFieldStateID, 1);

        //    // 再创建一个叫FeatureName的字符型属性，字符长度为50
        //    FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
        //    oFieldName.SetWidth(20);
        //    olayer1.CreateField(oFieldName, 1);

        //    // 再创建一个叫FeatureName的字符型属性，字符长度为50
        //    FieldDefn oFieldTime = new FieldDefn("Time", FieldType.OFTString);
        //    oFieldTime.SetWidth(20);
        //    olayer1.CreateField(oFieldTime, 1);

        //    // 再创建一个叫FeatureName的字符型属性，字符长度为50
        //    FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
        //    oFieldLongTime.SetWidth(20);
        //    olayer1.CreateField(oFieldLongTime, 1);

        //    //创建x坐标字段
        //    FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
        //    oFieldMinLog.SetWidth(20);
        //    oFieldMinLog.SetPrecision(8);
        //    olayer1.CreateField(oFieldMinLog, 1);
        //    //创建y坐标字段
        //    FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
        //    oFieldMinLat.SetWidth(20);
        //    oFieldMinLat.SetPrecision(8);
        //    olayer1.CreateField(oFieldMinLat, 1);
        //    //创建z坐标字段
        //    FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
        //    oFieldMaxLog.SetWidth(20);
        //    oFieldMaxLog.SetPrecision(8);
        //    olayer1.CreateField(oFieldMaxLog, 1);
        //    //创建z坐标字段
        //    FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
        //    oFieldMaxLat.SetWidth(20);
        //    oFieldMaxLat.SetPrecision(8);
        //    olayer1.CreateField(oFieldMaxLat, 1);

        //    //创建area字段
        //    FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
        //    oFieldArea.SetWidth(20);
        //    oFieldArea.SetPrecision(8);
        //    olayer1.CreateField(oFieldArea, 1);

        //    //创建平均降雨量字段
        //    FieldDefn oFieldAvgRainFall = new FieldDefn("IMean", FieldType.OFTReal);
        //    oFieldAvgRainFall.SetWidth(20);
        //    oFieldAvgRainFall.SetPrecision(8);
        //    olayer1.CreateField(oFieldAvgRainFall, 1);

        //    ////创建体积字段
        //    //FieldDefn oFieldVolume = new FieldDefn("Volume", FieldType.OFTReal);
        //    //oFieldVolume.SetWidth(20);
        //    //oFieldVolume.SetPrecision(8);
        //    //olayer1.CreateField(oFieldVolume, 1);

        //    //创建最大降雨量字段
        //    FieldDefn oFieldMaxRainFall = new FieldDefn("IMax", FieldType.OFTReal);
        //    oFieldMaxRainFall.SetWidth(20);
        //    oFieldMaxRainFall.SetPrecision(8);
        //    olayer1.CreateField(oFieldMaxRainFall, 1);

        //    //创建最大降雨量字段
        //    FieldDefn oFieldMinRainFall = new FieldDefn("IMin", FieldType.OFTReal);
        //    oFieldMinRainFall.SetWidth(20);
        //    oFieldMinRainFall.SetPrecision(8);
        //    olayer1.CreateField(oFieldMinRainFall, 1);

        //    //创建最大降雨量字段
        //    FieldDefn oFieldUSpeed = new FieldDefn("USpeed", FieldType.OFTReal);
        //    oFieldUSpeed.SetWidth(20);
        //    oFieldUSpeed.SetPrecision(8);
        //    olayer1.CreateField(oFieldUSpeed, 1);

        //    //创建最大降雨量字段
        //    FieldDefn oFieldVSpeed = new FieldDefn("VSpeed", FieldType.OFTReal);
        //    oFieldVSpeed.SetWidth(20);
        //    oFieldVSpeed.SetPrecision(8);
        //    olayer1.CreateField(oFieldVSpeed, 1);

        //    ////创建Power字段
        //    //FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTInteger);
        //    //olayer1.CreateField(oFieldPower, 1);

        //    //创建周长字段
        //    FieldDefn oFieldLength = new FieldDefn("Length", FieldType.OFTReal);
        //    oFieldLength.SetWidth(20);
        //    oFieldLength.SetPrecision(8);
        //    olayer1.CreateField(oFieldLength, 1);

        //    //创建质心字段
        //    FieldDefn oFieldLogCore = new FieldDefn("CoreLog", FieldType.OFTReal);
        //    oFieldLogCore.SetWidth(20);
        //    oFieldLogCore.SetPrecision(8);
        //    olayer1.CreateField(oFieldLogCore, 1);

        //    //创建质心字段
        //    FieldDefn oFieldLatCore = new FieldDefn("CoreLat", FieldType.OFTReal);
        //    oFieldLatCore.SetWidth(20);
        //    oFieldLatCore.SetPrecision(8);
        //    olayer1.CreateField(oFieldLatCore, 1);

        //    //创建形状系数字段
        //    FieldDefn oFieldSI = new FieldDefn("SI", FieldType.OFTReal);
        //    oFieldSI.SetWidth(20);
        //    oFieldSI.SetPrecision(8);
        //    olayer1.CreateField(oFieldSI, 1);

        //    //创建最大长度字段
        //    FieldDefn oFieldLMax = new FieldDefn("LMax", FieldType.OFTReal);
        //    oFieldLMax.SetWidth(20);
        //    oFieldLMax.SetPrecision(8);
        //    olayer1.CreateField(oFieldLMax, 1);

        //    //创建最大宽度字段
        //    FieldDefn oFieldWMax = new FieldDefn("WMax", FieldType.OFTReal);
        //    oFieldWMax.SetWidth(20);
        //    oFieldWMax.SetPrecision(8);
        //    olayer1.CreateField(oFieldWMax, 1);

        //    //创建偏心率字段
        //    FieldDefn oFieldERatio = new FieldDefn("ERatio", FieldType.OFTReal);
        //    oFieldERatio.SetWidth(20);
        //    oFieldERatio.SetPrecision(8);
        //    olayer1.CreateField(oFieldERatio, 1);

        //    //创建矩形度字段
        //    FieldDefn oFieldRecDeg = new FieldDefn("RecDeg", FieldType.OFTReal);
        //    oFieldRecDeg.SetWidth(20);
        //    oFieldRecDeg.SetPrecision(8);
        //    olayer1.CreateField(oFieldRecDeg, 1);

        //    //创建圆形度字段
        //    FieldDefn oFieldSphDeg = new FieldDefn("SphDeg", FieldType.OFTReal);
        //    oFieldSphDeg.SetWidth(20);
        //    oFieldSphDeg.SetPrecision(8);
        //    olayer1.CreateField(oFieldSphDeg, 1);

        //    //创建最小外接矩形点1
        //    FieldDefn oFieldMinRecP1X = new FieldDefn("RecP1X", FieldType.OFTString);
        //    oFieldMinRecP1X.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP1X, 1);

        //    //创建最小外接矩形点1
        //    FieldDefn oFieldMinRecP1Y = new FieldDefn("RecP1Y", FieldType.OFTString);
        //    oFieldMinRecP1Y.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP1Y, 1);

        //    //创建最小外接矩形点2
        //    FieldDefn oFieldMinRecP2X = new FieldDefn("RecP2X", FieldType.OFTString);
        //    oFieldMinRecP2X.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP2X, 1);

        //    //创建最小外接矩形点2
        //    FieldDefn oFieldMinRecP2Y = new FieldDefn("RecP2Y", FieldType.OFTString);
        //    oFieldMinRecP2Y.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP2Y, 1);

        //    //创建最小外接矩形点3
        //    FieldDefn oFieldMinRecP3X = new FieldDefn("RecP3X", FieldType.OFTString);
        //    oFieldMinRecP3X.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP3X, 1);

        //    //创建最小外接矩形点3
        //    FieldDefn oFieldMinRecP3Y = new FieldDefn("RecP3Y", FieldType.OFTString);
        //    oFieldMinRecP3Y.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP3Y, 1);

        //    //创建最小外接矩形点4
        //    FieldDefn oFieldMinRecP4X = new FieldDefn("RecP4X", FieldType.OFTString);
        //    oFieldMinRecP4X.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP4X, 1);

        //    //创建最小外接矩形点4
        //    FieldDefn oFieldMinRecP4Y = new FieldDefn("RecP4Y", FieldType.OFTString);
        //    oFieldMinRecP4Y.SetWidth(20);
        //    olayer1.CreateField(oFieldMinRecP4Y, 1);

        //    ////创建形状系数字段
        //    //FieldDefn oFieldP1 = new FieldDefn("P1", FieldType.OFTReal);
        //    //oFieldP1.SetWidth(10);
        //    //oFieldP1.SetPrecision(8);
        //    //olayer1.CreateField(oFieldP1, 1);

        //    ////创建形状系数字段
        //    //FieldDefn oFieldP2 = new FieldDefn("P2", FieldType.OFTReal);
        //    //oFieldP2.SetWidth(10);
        //    //oFieldP2.SetPrecision(8);
        //    //olayer1.CreateField(oFieldP2, 1);

        //    ////创建形状系数字段
        //    //FieldDefn oFieldP3 = new FieldDefn("P3", FieldType.OFTReal);
        //    //oFieldP3.SetWidth(10);
        //    //oFieldP3.SetPrecision(8);
        //    //olayer1.CreateField(oFieldP3, 1);

        //    ////创建形状系数字段
        //    //FieldDefn oFieldP4 = new FieldDefn("P4", FieldType.OFTReal);
        //    //oFieldP4.SetWidth(10);
        //    //oFieldP4.SetPrecision(8);
        //    //olayer1.CreateField(oFieldP4, 1);
        //    #endregion

        //    //写入数据
        //    FeatureDefn oDefn = olayer1.GetLayerDefn();

        //    foreach (Polygon polygon in polygons)
        //    {//写出每个多边形
        //        Feature oFeature = new Feature(oDefn);
        //        oFeature.SetField(0, polygon.id);
        //        //oFeature.SetField(1, polygon.stormID);
        //        oFeature.SetField(3, "MarineHeatwave");

        //        oFeature.SetField(4, startTime.ToString());
        //        DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
        //        long timeStamp = (long)(startTime - time1970).TotalSeconds; // 相差秒数
        //        oFeature.SetField(5, timeStamp.ToString());

        //        //计算经纬度
        //        double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
        //        double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
        //        double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
        //        double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

        //        //计算经纬度
        //        double CoreLog = startLog + (polygon.coreCol + 0.5) * resolution;//质心经度
        //        double CoreLat = startLat + (row - polygon.coreRow - 0.5) * resolution;//质心纬度

        //        //	形状系数（SI）：面积（A）/周长（P）
        //        double si = (4 * Math.Sqrt(polygon.area)) / polygon.length;
        //        double eRatio = polygon.minRec.width / polygon.minRec.length;
        //        double recDeg = polygon.area / ((polygon.minRec.length * polygon.minRec.width) * 123.93 * Math.Cos((maxLat + minLat) / 2 * Math.PI / 180));//最小外包矩形面积为近似计算
        //        double sphDeg = polygon.maxInCir.r / polygon.minOutCir.r;

        //        oFeature.SetField(6, minLog);
        //        oFeature.SetField(7, minLat);
        //        oFeature.SetField(8, maxLog);
        //        oFeature.SetField(9, maxLat);

        //        oFeature.SetField(10, polygon.area);
        //        oFeature.SetField(11, polygon.intensityMean);
        //        //oFeature.SetField(12, polygon.volume);
        //        oFeature.SetField(12, polygon.intensityMax);
        //        oFeature.SetField(13, polygon.intensityMin);
        //        oFeature.SetField(14, polygon.uSpeed);
        //        oFeature.SetField(15, polygon.vSpeed);
        //        //oFeature.SetField(15, polygon.power);
        //        oFeature.SetField(16, polygon.length);
        //        oFeature.SetField(17, CoreLog);
        //        oFeature.SetField(18, CoreLat);
        //        oFeature.SetField(19, si);
        //        oFeature.SetField(20, polygon.minRec.length * resolution);
        //        oFeature.SetField(21, polygon.minRec.width * resolution);
        //        oFeature.SetField(22, eRatio);
        //        oFeature.SetField(23, recDeg);
        //        oFeature.SetField(24, sphDeg);

        //        double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
        //        double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
        //        double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
        //        double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

        //        oFeature.SetField(25, p1t[0]);
        //        oFeature.SetField(26, p1t[1]);
        //        oFeature.SetField(27, p2t[0]);
        //        oFeature.SetField(28, p2t[1]);
        //        oFeature.SetField(29, p3t[0]);
        //        oFeature.SetField(30, p3t[1]);
        //        oFeature.SetField(31, p4t[0]);
        //        oFeature.SetField(32, p4t[1]);

        //        //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
        //        //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
        //        //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
        //        //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
        //        //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
        //        string polygonStr = "POLYGON (";
        //        foreach (Line line in polygon.lines)
        //        {//写出多边形中每条线
        //            polygonStr += "(";
        //            foreach (Node node in line.nodes)
        //            {
        //                int _row = node.row;//点行号
        //                int _col = node.col;//点列号

        //                double log = startLog + (_col + 1) * resolution;//经度
        //                double lat = startLat + (row - _row - 1) * resolution;//纬度

        //                polygonStr += log.ToString() + " " + lat.ToString();
        //                polygonStr += ",";
        //            }
        //            polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
        //            polygonStr += "),";
        //        }
        //        polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
        //        polygonStr += ")";
        //        Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
        //        //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
        //        //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
        //        //geoPolygon.AddGeometry(geoPolygon2);
        //        oFeature.SetGeometry(geoPolygon);
        //        olayer1.CreateFeature(oFeature);

        //        //释放资源
        //        geoPolygon.Dispose();
        //        oFeature.Dispose();
        //    }

        //    olayer1.Dispose();
        //    ds1.Dispose();

        //    TimeSpan ts = DateTime.Now - time1;
        //    Console.WriteLine(ts.TotalSeconds);

        //    #region 最小面积外包矩形输出shp(测试效果用)
        //    /*
        //    //保存shp
        //    Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
        //    // 为了使属性表字段支持中文，请添加下面这句
        //    Gdal.SetConfigOption("SHAPE_ENCODING", "");
        //    string strVectorFile2 = Path.GetDirectoryName(outPath)+"\\1\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinAreaRec.shp";
        //    Ogr.RegisterAll();

        //    string strDriver2 = "ESRI Shapefile";
        //    OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
        //    if (oDriver2 == null)
        //    {
        //        //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
        //        return;
        //    }
        //    DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
        //    if (ds2 == null)
        //    {
        //        //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
        //        return;
        //    }

        //    string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
        //    OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
        //    Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


        //    //写入数据
        //    FeatureDefn oDefn2 = olayer2.GetLayerDefn();

        //    foreach (Polygon polygon in polygons)
        //    {//写出每个多边形
        //        Feature oFeature = new Feature(oDefn2);
        //        //计算经纬度
        //        double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
        //        double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
        //        double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
        //        double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

        //        //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
        //        //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
        //        //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
        //        //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
        //        //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
        //        double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
        //        double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
        //        double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
        //        double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

        //        string polygonStr = "POLYGON (("+ p1t[0] + " " + p1t[1] + "," + p2t[0] + " " + p2t[1] + "," + p4t[0] + " " + p4t[1] + "," + p3t[0] + " " + p3t[1] + "," + p1t[0] + " " + p1t[1]+"))";
        //        Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
        //        //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
        //        //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
        //        //geoPolygon.AddGeometry(geoPolygon2);
        //        oFeature.SetGeometry(geoPolygon);
        //        olayer2.CreateFeature(oFeature);

        //        //释放资源
        //        geoPolygon.Dispose();
        //        oFeature.Dispose();
        //    }

        //    olayer2.Dispose();
        //    ds2.Dispose();
        //    */
        //    #endregion

        //    #region 最小面积外接圆输出shp(测试效果用)
        //    /*
        //    //保存shp
        //    Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
        //    // 为了使属性表字段支持中文，请添加下面这句
        //    Gdal.SetConfigOption("SHAPE_ENCODING", "");
        //    string strVectorFile2 = Path.GetDirectoryName(outPath)+"\\1\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinOutCir.shp";
        //    Ogr.RegisterAll();

        //    string strDriver2 = "ESRI Shapefile";
        //    OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
        //    if (oDriver2 == null)
        //    {
        //        //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
        //        return;
        //    }
        //    DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
        //    if (ds2 == null)
        //    {
        //        //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
        //        return;
        //    }

        //    string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
        //    OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
        //    Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


        //    //写入数据
        //    FeatureDefn oDefn2 = olayer2.GetLayerDefn();

        //    foreach (Polygon polygon in polygons)
        //    {//写出每个多边形
        //        Feature oFeature = new Feature(oDefn2);
        //        //计算经纬度
        //        //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
        //        //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
        //        //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
        //        //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

        //        //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
        //        //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
        //        //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
        //        //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
        //        //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

        //        string polygonStr = "POLYGON ((";
        //        double cx = polygon.minOutCir.x;
        //        double cy = polygon.minOutCir.y;
        //        double cr = polygon.minOutCir.r;
        //        for (int i=0;i<=360;i++)
        //        {
        //            double de = i * Math.PI / 180;//弧度制
        //            double x = cx + cr*Math.Sin(de);
        //            double y = cy + cr * Math.Cos(de);

        //            double log= startLog + (y + 1) * resolution;//经度
        //            double lat = startLat + (row - x - 1) * resolution;//纬度

        //            polygonStr += log + " " + lat + ",";
        //        }


        //        polygonStr=polygonStr.TrimEnd(',');
        //        polygonStr+= "))";
        //        Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
        //        //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
        //        //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
        //        //geoPolygon.AddGeometry(geoPolygon2);
        //        oFeature.SetGeometry(geoPolygon);
        //        olayer2.CreateFeature(oFeature);

        //        //释放资源
        //        geoPolygon.Dispose();
        //        oFeature.Dispose();
        //    }

        //    olayer2.Dispose();
        //    ds2.Dispose();
        //    */
        //    #endregion

        //    #region 最大面积内切圆输出shp(测试效果用)
        //    /*
        //    //保存shp
        //    Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
        //    // 为了使属性表字段支持中文，请添加下面这句
        //    Gdal.SetConfigOption("SHAPE_ENCODING", "");
        //    string strVectorFile2 = Path.GetDirectoryName(outPath) + "\\1\\" + Path.GetFileNameWithoutExtension(outPath) + "_MaxInCir.shp";
        //    Ogr.RegisterAll();

        //    string strDriver2 = "ESRI Shapefile";
        //    OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
        //    if (oDriver2 == null)
        //    {
        //        //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
        //        return;
        //    }
        //    DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
        //    if (ds2 == null)
        //    {
        //        //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
        //        return;
        //    }

        //    string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
        //    OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
        //    Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


        //    //写入数据
        //    FeatureDefn oDefn2 = olayer2.GetLayerDefn();

        //    foreach (Polygon polygon in polygons)
        //    {//写出每个多边形
        //        Feature oFeature = new Feature(oDefn2);
        //        //计算经纬度
        //        //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
        //        //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
        //        //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
        //        //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

        //        //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
        //        //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
        //        //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
        //        //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
        //        //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

        //        string polygonStr = "POLYGON ((";
        //        double cx = polygon.maxInCir.x;
        //        double cy = polygon.maxInCir.y;
        //        double cr = polygon.maxInCir.r;
        //        for (int i = 0; i <= 360; i++)
        //        {
        //            double de = i * Math.PI / 180;//弧度制
        //            double x = cx + cr * Math.Sin(de);
        //            double y = cy + cr * Math.Cos(de);

        //            double log = startLog + (y + 1) * resolution;//经度
        //            double lat = startLat + (row - x - 1) * resolution;//纬度

        //            polygonStr += log + " " + lat + ",";
        //        }


        //        polygonStr = polygonStr.TrimEnd(',');
        //        polygonStr += "))";
        //        Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
        //        //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
        //        //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
        //        //geoPolygon.AddGeometry(geoPolygon2);
        //        oFeature.SetGeometry(geoPolygon);
        //        olayer2.CreateFeature(oFeature);

        //        //释放资源
        //        geoPolygon.Dispose();
        //        oFeature.Dispose();
        //    }

        //    olayer2.Dispose();
        //    ds2.Dispose();
        //    */
        //    #endregion
        //}
        public static void MarineHeatwavesTifToShp(string timeFilePath, string uFilePath, string vFilePath, string outPath, double timeCell)
        {
            DateTime time1 = DateTime.Now;
            //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
            //Gdal.AllRegister();//注册所有的格式驱动
            //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

            //打开hdf文件
            //Dataset oriDs = Gdal.Open(oriFilePath, Access.GA_ReadOnly);
            Dataset timeDs = Gdal.Open(timeFilePath, Access.GA_ReadOnly);
            Dataset uDs = Gdal.Open(uFilePath, Access.GA_ReadOnly);
            Dataset vDs = Gdal.Open(vFilePath, Access.GA_ReadOnly);
            int col = timeDs.RasterXSize;//列数
            int row = timeDs.RasterYSize;//行数
            //Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
            Band timeBand1 = timeDs.GetRasterBand(1);//读取波段
            Band uBand1 = uDs.GetRasterBand(1);//读取波段
            Band vBand1 = vDs.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            timeDs.GetGeoTransform(argout);//读取地理坐标信息

            string[] metadatas = timeDs.GetMetadata("");//获取元数据
            double startLog = 0.0;//起始经度
            double startLat = 0.0;//起始维度
            double endLog = 0.0;//结束经度
            double endLat = 0.0;//结束维度
            double scale = 0.0;//比例
            int fillValue = 0;
            double resolution = 0.0;//分辨率

            string fileName = Path.GetFileNameWithoutExtension(timeFilePath);
            DateTime startTime = DateTime.ParseExact(fileName, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);//起始时间 
            //DateTime startTime = DateTime.Now;//起始时间 

            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        scale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "FillValue":
                        fillValue = Convert.ToInt32(mdArr[1]);
                        break;
                    case "Resolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }

            string[] uMetadatas = uDs.GetMetadata("");//获取元数据
            double uSacle = 0.0;
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "Scale":
                        uSacle = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }

            string[] vMetadatas = vDs.GetMetadata("");//获取元数据
            double vSacle = 0.0;
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "Scale":
                        vSacle = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }

            //double[] oriData = new double[row * col];//存储
            int[] timeData = new int[row * col];//存储
            int[] uData = new int[row * col];//存储
            int[] vData = new int[row * col];//存储
            //oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
            timeBand1.ReadRaster(0, 0, col, row, timeData, col, row, 0, 0);//读取数据
            uBand1.ReadRaster(0, 0, col, row, uData, col, row, 0, 0);//读取数据
            vBand1.ReadRaster(0, 0, col, row, vData, col, row, 0, 0);//读取数据
            //oriDs.Dispose();
            timeBand1.Dispose();
            uBand1.Dispose();
            vBand1.Dispose();
            timeDs.Dispose();
            uDs.Dispose();
            vDs.Dispose();

            //double[,] oriImg = new double[row, col];//二维数组，边缘不处理
            int[,] timeImg = new int[row, col];//二维数组，边缘不处理
            int[,] uImg = new int[row, col];//二维数组，边缘不处理
            int[,] vImg = new int[row, col];//二维数组，边缘不处理
            for (int i = 0; i < timeData.Length; i++)
            {//将一维数组转换为二维，方便处理
                int _rowNow = i / col;//行号
                int _colNow = i % col;//列号
                if (_rowNow == 0 || _colNow == 0 || _rowNow == timeImg.GetLength(0) - 1 || _colNow == timeImg.GetLength(1) - 1) continue;//边界点不添加

                //oriImg[_rowNow, _colNow] = oriData[i] * scale;//乘以系数
                if (timeData[i] > 0)
                {
                    timeImg[_rowNow, _colNow] = timeData[i];
                }
                if (uData[i] != -9999) uImg[_rowNow, _colNow] = uData[i];
                if (vData[i] != -9999) vImg[_rowNow, _colNow] = vData[i];
            }

            //寻找结点
            List<Node> nodes = new List<Node>();//结点链表
            for (int i = 0; i < timeImg.GetLength(0) - 1; i++)
            {//行循环
                for (int j = 0; j < timeImg.GetLength(1) - 1; j++)
                {
                    int ltv = timeImg[i, j];//左上角栅格值
                    int rtv = timeImg[i, j + 1];//右上角栅格值
                    int lbv = timeImg[i + 1, j];//左下角栅格值
                    int rbv = timeImg[i + 1, j + 1];//右下角栅格值
                    if ((ltv == 0 && rtv > 0 && lbv > 0 && rbv > 0) || (ltv > 0 && rtv == 0 && lbv == 0 && rbv == 0))
                    {//左上角不同，左和上方向
                        string dir1 = "l";//左方向
                        string dir2 = "t";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 1;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    else if ((rtv == 0 && ltv > 0 && lbv > 0 && rbv > 0) || (rtv > 0 && ltv == 0 && lbv == 0 && rbv == 0))
                    {//右上角不同，上和右方向
                        string dir1 = "t";//左方向
                        string dir2 = "r";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 2;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    else if ((lbv == 0 && ltv > 0 && rtv > 0 && rbv > 0) || (lbv > 0 && ltv == 0 && rtv == 0 && rbv == 0))
                    {//左下角不同，下和左方向
                        string dir1 = "b";//左方向
                        string dir2 = "l";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 3;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    else if ((rbv == 0 && ltv > 0 && rtv > 0 && lbv > 0) || (rbv > 0 && ltv == 0 && rtv == 0 && lbv == 0))
                    {//右下角不同，右和下方向
                        string dir1 = "r";//左方向
                        string dir2 = "b";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 4;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    else if ((ltv == 0 && rbv == 0 && rtv > 0 && lbv > 0) || (ltv > 0 && rbv > 0 && rtv == 0 && lbv == 0))
                    {//对角相等，相邻不等
                        string dir1 = "n";//左方向
                        string dir2 = "n";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        if (ltv > 0)
                        {//左上角和右下角值大于零
                            node.type = 5;
                            node.power = ltv;
                            node.stormID = timeImg[i, j];//左上角栅格值
                        }
                        else
                        {//右上角和左下角值大于零
                            node.type = 6;
                            node.power = rtv;
                            node.stormID = timeImg[i, j + 1];//右上角栅格值
                        }
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                }
            }
            Node[] nodeArr = nodes.ToArray();

            List<Line> lines = new List<Line>();//所有线
            //结点连成线
            for (int i = 0; i < nodeArr.Length; i++)
            {//寻找每一条线
                if (nodeArr[i].isUsed == true || nodeArr[i].type > 4) continue;//被使用，跳出本次循环
                Node headNode = nodeArr[i];//头结点
                headNode.isUsed = true;//记录被使用
                headNode.outDir = headNode.dir1;//记录出去的方向
                nodeArr[i] = headNode;//进行保存
                Line line = new Line();//新建一条线
                int minRow = headNode.row;
                int minCol = headNode.col;
                int maxRow = headNode.row;
                int maxCol = headNode.col;
                //line.nodes.Add(headNode);//
                List<Node> lineNodes = new List<Node>();//线中所有点
                lineNodes.Add(headNode);//将第一个点添加进去
                Node tailNode = headNode;//尾结点
                //Node nextNode = new Node();//用来记录找到的下一个结点
                double lineLength = 0.0;//线的长度
                do
                {
                    int startNodeRow = tailNode.row;
                    int startNodeCol = tailNode.col;
                    tailNode = GetNextNode(tailNode, ref nodeArr, ref minRow, ref minCol, ref maxRow, ref maxCol);

                    //计算周长
                    double logInterval = Math.Abs(tailNode.col - startNodeCol) * resolution;//经度差值
                    if (logInterval == 0.0)
                    {//竖线
                        double latInterval = Math.Abs(tailNode.row - startNodeRow) * resolution;//纬度差值
                        double _length = Earth.ML * 0.001 * latInterval / 180.0;//经线长度为20017km（百度百科），每条经线长度都相同
                        lineLength += _length;
                    }
                    else
                    {//横线
                        double lineLat = startLat + (row - tailNode.row) * resolution;//纬度
                        double equatorLengthNow = Earth.EC * 0.001 * Math.Cos(lineLat * Math.PI / 180.0);//当前纬线周长
                        double _length = equatorLengthNow * logInterval / 360.0;
                        lineLength += _length;
                    }

                    lineNodes.Add(tailNode);//将点添加进去
                } while (tailNode.id != headNode.id);

                //判断是内环还是外环
                Node firstNode = lineNodes[0];//第一个结点
                if (firstNode.type == 1)
                {
                    if (timeImg[firstNode.row, firstNode.col] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 2)
                {
                    if (timeImg[firstNode.row, firstNode.col + 1] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 3)
                {
                    if (timeImg[firstNode.row + 1, firstNode.col] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 4)
                {
                    if (timeImg[firstNode.row + 1, firstNode.col + 1] > 0)//右下角栅格值
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }

                line.id = lines.Count;
                line.nodes = lineNodes;//进行保存
                line.minRow = minRow;
                line.minCol = minCol;
                line.maxRow = maxRow;
                line.maxCol = maxCol;
                line.power = headNode.power;//值
                line.length = lineLength;//线的长度
                if (line.type == 1 && line.length < 1000) continue;//周长过小剔除
                lines.Add(line);
            }
            Line[] lineArr = lines.ToArray();

            //线构成面
            List<Polygon> polygons = new List<Polygon>();//所有面
            for (int i = 0; i < lineArr.Length; i++)
            {//循环每一条线
                if (lineArr[i].type != 0) continue;//不是外环，退出本次循环
                Polygon polygon = new Polygon();
                List<Line> pLines = new List<Line>();
                pLines.Add(lineArr[i]);//添加外环
                polygon.id = polygons.Count;
                polygon.minCol = lineArr[i].minCol;
                polygon.minRow = lineArr[i].minRow;
                polygon.maxCol = lineArr[i].maxCol;
                polygon.maxRow = lineArr[i].maxRow;
                polygon.power = lineArr[i].power;
                for (int j = i + 1; j < lineArr.Length; j++)
                {//寻找该外环包含的内环，内环肯定在后面
                    //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
                    if (lineArr[j].type == 0) continue;//外环
                    if (lineArr[j].minRow > lineArr[i].minRow && lineArr[j].minCol > lineArr[i].minCol && lineArr[j].maxRow < lineArr[i].maxRow && lineArr[j].maxCol < lineArr[i].maxCol)
                    {//邻接矩形包含，进一步判断
                        //只要一个点在外环内，就是内环
                        bool isIn = IsInPolygonNew(lineArr[j].nodes[0], lineArr[i].nodes);//判断是否在外环里面
                        if (isIn)
                        {//内环
                            Line line = lineArr[j];//取出线
                            line.nodes.Reverse();//顺序反转
                            //line.type = 1;
                            lineArr[j] = line;//保存修改后的
                            pLines.Add(line);//添加内环
                        }
                    }
                }
                polygon.lines = pLines;
                double volume = 0.0;//温度异常累加
                double intensityMean = 0.0;//平均温度异常
                double intensityMax = double.MinValue;//最大温度异常
                double intensityMin = double.MaxValue;//最小温度异常
                double uArea = 0.0;//经线方向速度与面积乘积
                double vArea = 0.0;//纬线方向速度与面积成绩
                //double avgRainfall = 0.0;//平均降雨量
                //double volume = 0.0;//累计降雨量
                //double maxRainfall = 0.0;//最大降雨量
                //double minRainfall = double.MaxValue;//最小降雨量
                double area = 0.0;//面积
                double _rowCore = 0.0;//重心行号中间量
                double _colCore = 0.0;//重心列号中间量
                for (int _row = polygon.minRow + 1; _row <= polygon.maxRow; _row++)
                {//行循环，行列号对应节点左上角栅格
                    double rasterStartLat = startLat + (row - _row - 1) * resolution;//栅格下边缘纬度
                    double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
                    double rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积
                    for (int _col = polygon.minCol + 1; _col <= polygon.maxCol; _col++)
                    {//列循环
                        //if (idImg[_row, _col] == polygon.stormID)
                        if (IsInPolygonNew(_row - 0.5f, _col - 0.5f, pLines[0].nodes))
                        {//当前暴雨对象
                            float _rowF = _row - 0.5f;//栅格行列号转为矢量行列号
                            float _colF = _col - 0.5f;
                            if (IsInPolygonNew(_rowF, _colF, polygon.lines[0].nodes))
                            {//包含关系
                                double uSpeed = uImg[_row, _col];
                                double vSpeed = vImg[_row, _col];
                                uArea += uSpeed * rasterArea;
                                vArea += vSpeed * rasterArea;
                                area += rasterArea;//增加面积
                                //double rainfall = oriImg[_row, _col] / timeCell;//换算为每小时
                                double intensity = timeImg[_row, _col];//每半小时
                                if (intensity > intensityMax) intensityMax = intensity;//最大降雨量
                                if (intensity < intensityMin) intensityMin = intensity;
                                double _volume = rasterArea * intensity;//暴雨总降雨量累加
                                volume += _volume;
                                //加权
                                _rowCore += _volume * _row;
                                _colCore += _volume * _col;
                            }
                        }
                    }
                }
                //平均
                polygon.coreRow = _rowCore / volume;
                polygon.coreCol = _colCore / volume;

                intensityMean = volume / area;//计算平均降雨量
                polygon.intensityMean = intensityMean * scale;//保存平均降雨量
                polygon.intensityMax = intensityMax * scale;//保存最大降雨量
                polygon.intensityMin = intensityMin * scale;//保存最大降雨量
                polygon.area = area;

                polygon.uSpeed = uSacle * uArea / area;
                polygon.vSpeed = vSacle * vArea / area;

                //volume = volume * 1000;//立方米
                //polygon.volume = volume;
                polygon.length = polygon.lines[0].length;//周长
                polygon.minRec = GetMinAreaRec(polygon);
                polygon.minOutCir = GetMinOutCir(polygon);
                polygon.maxInCir = GetMaxInCir(polygon, 0.5);
                if (volume > 0) polygons.Add(polygon);
            }

            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = outPath;
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);
            //接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("EventID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStateID = new FieldDefn("StateID", FieldType.OFTString);
            oFieldStateID.SetWidth(20);
            olayer1.CreateField(oFieldStateID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
            oFieldName.SetWidth(20);
            olayer1.CreateField(oFieldName, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldTime.SetWidth(20);
            olayer1.CreateField(oFieldTime, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            oFieldLongTime.SetWidth(20);
            olayer1.CreateField(oFieldLongTime, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
            oFieldMinLog.SetWidth(20);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建area字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            olayer1.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("IMean", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            ////创建体积字段
            //FieldDefn oFieldVolume = new FieldDefn("Volume", FieldType.OFTReal);
            //oFieldVolume.SetWidth(20);
            //oFieldVolume.SetPrecision(8);
            //olayer1.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("IMax", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMaxRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("IMin", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMinRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldUSpeed = new FieldDefn("USpeed", FieldType.OFTReal);
            oFieldUSpeed.SetWidth(20);
            oFieldUSpeed.SetPrecision(8);
            olayer1.CreateField(oFieldUSpeed, 1);

            //创建最大降雨量字段
            FieldDefn oFieldVSpeed = new FieldDefn("VSpeed", FieldType.OFTReal);
            oFieldVSpeed.SetWidth(20);
            oFieldVSpeed.SetPrecision(8);
            olayer1.CreateField(oFieldVSpeed, 1);

            ////创建Power字段
            //FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTInteger);
            //olayer1.CreateField(oFieldPower, 1);

            //创建周长字段
            FieldDefn oFieldLength = new FieldDefn("Length", FieldType.OFTReal);
            oFieldLength.SetWidth(20);
            oFieldLength.SetPrecision(8);
            olayer1.CreateField(oFieldLength, 1);

            //创建质心字段
            FieldDefn oFieldLogCore = new FieldDefn("CoreLog", FieldType.OFTReal);
            oFieldLogCore.SetWidth(20);
            oFieldLogCore.SetPrecision(8);
            olayer1.CreateField(oFieldLogCore, 1);

            //创建质心字段
            FieldDefn oFieldLatCore = new FieldDefn("CoreLat", FieldType.OFTReal);
            oFieldLatCore.SetWidth(20);
            oFieldLatCore.SetPrecision(8);
            olayer1.CreateField(oFieldLatCore, 1);

            //创建形状系数字段
            FieldDefn oFieldSI = new FieldDefn("SI", FieldType.OFTReal);
            oFieldSI.SetWidth(20);
            oFieldSI.SetPrecision(8);
            olayer1.CreateField(oFieldSI, 1);

            //创建最大长度字段
            FieldDefn oFieldLMax = new FieldDefn("LMax", FieldType.OFTReal);
            oFieldLMax.SetWidth(20);
            oFieldLMax.SetPrecision(8);
            olayer1.CreateField(oFieldLMax, 1);

            //创建最大宽度字段
            FieldDefn oFieldWMax = new FieldDefn("WMax", FieldType.OFTReal);
            oFieldWMax.SetWidth(20);
            oFieldWMax.SetPrecision(8);
            olayer1.CreateField(oFieldWMax, 1);

            //创建偏心率字段
            FieldDefn oFieldERatio = new FieldDefn("ERatio", FieldType.OFTReal);
            oFieldERatio.SetWidth(20);
            oFieldERatio.SetPrecision(8);
            olayer1.CreateField(oFieldERatio, 1);

            //创建矩形度字段
            FieldDefn oFieldRecDeg = new FieldDefn("RecDeg", FieldType.OFTReal);
            oFieldRecDeg.SetWidth(20);
            oFieldRecDeg.SetPrecision(8);
            olayer1.CreateField(oFieldRecDeg, 1);

            //创建圆形度字段
            FieldDefn oFieldSphDeg = new FieldDefn("SphDeg", FieldType.OFTReal);
            oFieldSphDeg.SetWidth(20);
            oFieldSphDeg.SetPrecision(8);
            olayer1.CreateField(oFieldSphDeg, 1);

            //创建最小外接矩形点1
            FieldDefn oFieldMinRecP1X = new FieldDefn("RecP1X", FieldType.OFTString);
            oFieldMinRecP1X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP1X, 1);

            //创建最小外接矩形点1
            FieldDefn oFieldMinRecP1Y = new FieldDefn("RecP1Y", FieldType.OFTString);
            oFieldMinRecP1Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP1Y, 1);

            //创建最小外接矩形点2
            FieldDefn oFieldMinRecP2X = new FieldDefn("RecP2X", FieldType.OFTString);
            oFieldMinRecP2X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP2X, 1);

            //创建最小外接矩形点2
            FieldDefn oFieldMinRecP2Y = new FieldDefn("RecP2Y", FieldType.OFTString);
            oFieldMinRecP2Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP2Y, 1);

            //创建最小外接矩形点3
            FieldDefn oFieldMinRecP3X = new FieldDefn("RecP3X", FieldType.OFTString);
            oFieldMinRecP3X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP3X, 1);

            //创建最小外接矩形点3
            FieldDefn oFieldMinRecP3Y = new FieldDefn("RecP3Y", FieldType.OFTString);
            oFieldMinRecP3Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP3Y, 1);

            //创建最小外接矩形点4
            FieldDefn oFieldMinRecP4X = new FieldDefn("RecP4X", FieldType.OFTString);
            oFieldMinRecP4X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP4X, 1);

            //创建最小外接矩形点4
            FieldDefn oFieldMinRecP4Y = new FieldDefn("RecP4Y", FieldType.OFTString);
            oFieldMinRecP4Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP4Y, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP1 = new FieldDefn("P1", FieldType.OFTReal);
            //oFieldP1.SetWidth(10);
            //oFieldP1.SetPrecision(8);
            //olayer1.CreateField(oFieldP1, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP2 = new FieldDefn("P2", FieldType.OFTReal);
            //oFieldP2.SetWidth(10);
            //oFieldP2.SetPrecision(8);
            //olayer1.CreateField(oFieldP2, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP3 = new FieldDefn("P3", FieldType.OFTReal);
            //oFieldP3.SetWidth(10);
            //oFieldP3.SetPrecision(8);
            //olayer1.CreateField(oFieldP3, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP4 = new FieldDefn("P4", FieldType.OFTReal);
            //oFieldP4.SetWidth(10);
            //oFieldP4.SetPrecision(8);
            //olayer1.CreateField(oFieldP4, 1);


            //写入数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, polygon.id);
                //oFeature.SetField(1, polygon.stormID);
                oFeature.SetField(3, "MarineHeatwave");

                oFeature.SetField(4, startTime.ToString());
                DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
                long timeStamp = (long)(startTime - time1970).TotalSeconds; // 相差秒数
                oFeature.SetField(5, timeStamp.ToString());

                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //计算经纬度
                double CoreLog = startLog + (polygon.coreCol + 0.5) * resolution;//质心经度
                double CoreLat = startLat + (row - polygon.coreRow - 0.5) * resolution;//质心纬度

                //	形状系数（SI）：面积（A）/周长（P）
                double si = (4 * Math.Sqrt(polygon.area)) / polygon.length;
                double eRatio = polygon.minRec.width / polygon.minRec.length;
                double recDeg = polygon.area / ((polygon.minRec.length * polygon.minRec.width) * 123.93 * Math.Cos((maxLat + minLat) / 2 * Math.PI / 180));//最小外包矩形面积为近似计算
                double sphDeg = polygon.maxInCir.r / polygon.minOutCir.r;

                oFeature.SetField(6, minLog);
                oFeature.SetField(7, minLat);
                oFeature.SetField(8, maxLog);
                oFeature.SetField(9, maxLat);

                oFeature.SetField(10, polygon.area);
                oFeature.SetField(11, polygon.intensityMean);
                //oFeature.SetField(12, polygon.volume);
                oFeature.SetField(12, polygon.intensityMax);
                oFeature.SetField(13, polygon.intensityMin);
                oFeature.SetField(14, polygon.uSpeed);
                oFeature.SetField(15, polygon.vSpeed);
                //oFeature.SetField(15, polygon.power);
                oFeature.SetField(16, polygon.length);
                oFeature.SetField(17, CoreLog);
                oFeature.SetField(18, CoreLat);
                oFeature.SetField(19, si);
                oFeature.SetField(20, polygon.minRec.length * resolution);
                oFeature.SetField(21, polygon.minRec.width * resolution);
                oFeature.SetField(22, eRatio);
                oFeature.SetField(23, recDeg);
                oFeature.SetField(24, sphDeg);

                double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
                double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
                double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
                double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

                oFeature.SetField(25, p1t[0]);
                oFeature.SetField(26, p1t[1]);
                oFeature.SetField(27, p2t[0]);
                oFeature.SetField(28, p2t[1]);
                oFeature.SetField(29, p3t[0]);
                oFeature.SetField(30, p3t[1]);
                oFeature.SetField(31, p4t[0]);
                oFeature.SetField(32, p4t[1]);

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "POLYGON (";
                foreach (Line line in polygon.lines)
                {//写出多边形中每条线
                    polygonStr += "(";
                    foreach (Node node in line.nodes)
                    {
                        int _row = node.row;//点行号
                        int _col = node.col;//点列号

                        double log = startLog + (_col + 1) * resolution;//经度
                        double lat = startLat + (row - _row - 1) * resolution;//纬度

                        polygonStr += log.ToString() + " " + lat.ToString();
                        polygonStr += ",";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer1.Dispose();
            ds1.Dispose();

            TimeSpan ts = DateTime.Now - time1;
            Console.WriteLine(ts.TotalSeconds);

            #region 最小面积外包矩形输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile2 = Path.GetDirectoryName(outPath)+"\\1\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinAreaRec.shp";
            Ogr.RegisterAll();

            string strDriver2 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
            if (oDriver2 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
            if (ds2 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
            Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn2 = olayer2.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn2);
                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
                double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
                double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
                double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

                string polygonStr = "POLYGON (("+ p1t[0] + " " + p1t[1] + "," + p2t[0] + " " + p2t[1] + "," + p3t[0] + " " + p3t[1] + "," + p4t[0] + " " + p4t[1] + "," + p1t[0] + " " + p1t[1]+"))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer2.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer2.Dispose();
            ds2.Dispose();
            */
            #endregion

            #region 最小面积外接圆输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile3 = Path.GetDirectoryName(outPath)+"\\2\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinOutCir.shp";
            Ogr.RegisterAll();

            string strDriver3 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver3 = Ogr.GetDriverByName(strDriver3);
            if (oDriver3 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds3 = oDriver3.CreateDataSource(strVectorFile3, null);
            if (ds3 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt3 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr3 = new OSGeo.OSR.SpatialReference(wkt3);
            Layer olayer3 = ds3.CreateLayer("PolygonLayer", sr3, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn3 = olayer3.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn3);
                //计算经纬度
                //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

                string polygonStr = "POLYGON ((";
                double cx = polygon.minOutCir.x;
                double cy = polygon.minOutCir.y;
                double cr = polygon.minOutCir.r;
                for (int i=0;i<=360;i++)
                {
                    double de = i * Math.PI / 180;//弧度制
                    double x = cx + cr*Math.Sin(de);
                    double y = cy + cr * Math.Cos(de);

                    double log= startLog + (y + 1) * resolution;//经度
                    double lat = startLat + (row - x - 1) * resolution;//纬度

                    polygonStr += log + " " + lat + ",";
                }


                polygonStr=polygonStr.TrimEnd(',');
                polygonStr+= "))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer3.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer3.Dispose();
            ds3.Dispose();
            */
            #endregion

            #region 最大面积内切圆输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile4 = Path.GetDirectoryName(outPath) + "\\3\\" + Path.GetFileNameWithoutExtension(outPath) + "_MaxInCir.shp";
            Ogr.RegisterAll();

            string strDriver4 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver4 = Ogr.GetDriverByName(strDriver4);
            if (oDriver4 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds4 = oDriver4.CreateDataSource(strVectorFile4, null);
            if (ds4 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt4 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr4 = new OSGeo.OSR.SpatialReference(wkt4);
            Layer olayer4 = ds4.CreateLayer("PolygonLayer", sr4, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn4 = olayer4.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn4);
                //计算经纬度
                //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

                string polygonStr = "POLYGON ((";
                double cx = polygon.maxInCir.x;
                double cy = polygon.maxInCir.y;
                double cr = polygon.maxInCir.r;
                for (int i = 0; i <= 360; i++)
                {
                    double de = i * Math.PI / 180;//弧度制
                    double x = cx + cr * Math.Sin(de);
                    double y = cy + cr * Math.Cos(de);

                    double log = startLog + (y + 1) * resolution;//经度
                    double lat = startLat + (row - x - 1) * resolution;//纬度

                    polygonStr += log + " " + lat + ",";
                }


                polygonStr = polygonStr.TrimEnd(',');
                polygonStr += "))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer4.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer4.Dispose();
            ds4.Dispose();
            */
            #endregion
        }
        
        /// <summary>
        /// 海洋表面温度图像的矢量提取SST
        /// </summary>
        /// <param name="oriFilePath"></param>
        /// <param name="valueScale"></param>
        /// <param name="timeCell"></param>
        /// <param name="idPath"></param>
        /// <param name="outPath"></param>
        public static void SST_TifToShp(string timeFilePath, string uFilePath, string vFilePath, string outPath, double timeCell)
        {
            DateTime time1 = DateTime.Now;
            //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
            //Gdal.AllRegister();//注册所有的格式驱动
            //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

            //打开hdf文件
            //Dataset oriDs = Gdal.Open(oriFilePath, Access.GA_ReadOnly);
            Dataset timeDs = Gdal.Open(timeFilePath, Access.GA_ReadOnly);
            Dataset uDs = Gdal.Open(uFilePath, Access.GA_ReadOnly);
            Dataset vDs = Gdal.Open(vFilePath, Access.GA_ReadOnly);
            int col = timeDs.RasterXSize;//列数
            int row = timeDs.RasterYSize;//行数
            //Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
            Band timeBand1 = timeDs.GetRasterBand(1);//读取波段
            Band uBand1 = uDs.GetRasterBand(1);//读取波段
            Band vBand1 = vDs.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            timeDs.GetGeoTransform(argout);//读取地理坐标信息

            string[] metadatas = timeDs.GetMetadata("");//获取元数据
            double startLog = 0.0;//起始经度
            double startLat = 0.0;//起始维度
            double endLog = 0.0;//结束经度
            double endLat = 0.0;//结束维度
            double scale = 0.0;//比例
            int fillValue = 0;
            double resolution = 0.0;//分辨率

            //string fileName = Path.GetFileNameWithoutExtension(timeFilePath);
            //DateTime startTime = DateTime.ParseExact(fileName, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);//起始时间 
            string fileName = Path.GetFileNameWithoutExtension(timeFilePath).Substring(0, 6);
            DateTime startTime = DateTime.ParseExact(fileName, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);//起始时间 
            //DateTime startTime = DateTime.Now;//起始时间 

            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        scale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "FillValue":
                        //fillValue = Convert.ToInt32(mdArr[1]);
                        fillValue = (int)Convert.ToDouble(mdArr[1]);
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }

            string[] uMetadatas = uDs.GetMetadata("");//获取元数据
            double uSacle = 0.0;
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "Scale":
                        uSacle = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }

            string[] vMetadatas = vDs.GetMetadata("");//获取元数据
            double vSacle = 0.0;
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "Scale":
                        vSacle = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }

            //double[] oriData = new double[row * col];//存储
            int[] timeData = new int[row * col];//存储
            int[] uData = new int[row * col];//存储
            int[] vData = new int[row * col];//存储
            //oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
            timeBand1.ReadRaster(0, 0, col, row, timeData, col, row, 0, 0);//读取数据
            uBand1.ReadRaster(0, 0, col, row, uData, col, row, 0, 0);//读取数据
            vBand1.ReadRaster(0, 0, col, row, vData, col, row, 0, 0);//读取数据
            //oriDs.Dispose();
            timeBand1.Dispose();
            uBand1.Dispose();
            vBand1.Dispose();
            timeDs.Dispose();
            uDs.Dispose();
            vDs.Dispose();

            //double[,] oriImg = new double[row, col];//二维数组，边缘不处理
            int[,] timeImg = new int[row, col];//二维数组，边缘不处理
            int[,] uImg = new int[row, col];//二维数组，边缘不处理
            int[,] vImg = new int[row, col];//二维数组，边缘不处理
            for (int i = 0; i < timeData.Length; i++)
            {//将一维数组转换为二维，方便处理
                int _rowNow = i / col;//行号
                int _colNow = i % col;//列号
                if (_rowNow == 0 || _colNow == 0 || _rowNow == timeImg.GetLength(0) - 1 || _colNow == timeImg.GetLength(1) - 1) continue;//边界点不添加

                //oriImg[_rowNow, _colNow] = oriData[i] * scale;//乘以系数
                //if (timeData[i] > 0)
                //{
                //    timeImg[_rowNow, _colNow] = timeData[i];
                //}
                if (timeData[i] != -9999) timeImg[_rowNow, _colNow] = timeData[i];
                if (uData[i] != -9999) uImg[_rowNow, _colNow] = uData[i];
                if (vData[i] != -9999) vImg[_rowNow, _colNow] = vData[i];
            }

            #region 寻找结点
            List<Node> nodes = new List<Node>();//结点链表
            for (int i = 0; i < timeImg.GetLength(0) - 1; i++)
            {//行循环
                for (int j = 0; j < timeImg.GetLength(1) - 1; j++)
                {
                    int ltv = timeImg[i, j];//左上角栅格值
                    int rtv = timeImg[i, j + 1];//右上角栅格值
                    int lbv = timeImg[i + 1, j];//左下角栅格值
                    int rbv = timeImg[i + 1, j + 1];//右下角栅格值
                    //if ((ltv == 0 && rtv > 0 && lbv > 0 && rbv > 0) || (ltv > 0 && rtv == 0 && lbv == 0 && rbv == 0))
                    if ((ltv == 0 && rtv != 0 && lbv != 0 && rbv != 0) || (ltv != 0 && rtv == 0 && lbv == 0 && rbv == 0))
                    {//左上角不同，左和上方向
                        string dir1 = "l";//左方向
                        string dir2 = "t";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 1;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    //else if ((rtv == 0 && ltv > 0 && lbv > 0 && rbv > 0) || (rtv > 0 && ltv == 0 && lbv == 0 && rbv == 0))
                    else if ((rtv == 0 && ltv != 0 && lbv != 0 && rbv != 0) || (rtv != 0 && ltv == 0 && lbv == 0 && rbv == 0))
                    {//右上角不同，上和右方向
                        string dir1 = "t";//左方向
                        string dir2 = "r";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 2;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    //else if ((lbv == 0 && ltv > 0 && rtv > 0 && rbv > 0) || (lbv > 0 && ltv == 0 && rtv == 0 && rbv == 0))
                    else if ((lbv == 0 && ltv != 0 && rtv != 0 && rbv != 0) || (lbv != 0 && ltv == 0 && rtv == 0 && rbv == 0))
                    {//左下角不同，下和左方向
                        string dir1 = "b";//左方向
                        string dir2 = "l";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 3;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    //else if ((rbv == 0 && ltv > 0 && rtv > 0 && lbv > 0) || (rbv > 0 && ltv == 0 && rtv == 0 && lbv == 0))
                    else if ((rbv == 0 && ltv != 0 && rtv != 0 && lbv != 0) || (rbv != 0 && ltv == 0 && rtv == 0 && lbv == 0))
                    {//右下角不同，右和下方向
                        string dir1 = "r";//左方向
                        string dir2 = "b";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 4;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                    //else if ((ltv == 0 && rbv == 0 && rtv > 0 && lbv > 0) || (ltv > 0 && rbv > 0 && rtv == 0 && lbv == 0))
                    else if ((ltv == 0 && rbv == 0 && rtv != 0 && lbv != 0) || (ltv != 0 && rbv != 0 && rtv == 0 && lbv == 0))
                    {//对角相等，相邻不等
                        string dir1 = "n";//左方向
                        string dir2 = "n";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        //if (ltv > 0)
                        if (ltv != 0)
                        {//左上角和右下角值大于零
                            node.type = 5;
                            node.power = ltv;
                            node.stormID = timeImg[i, j];//左上角栅格值
                        }
                        else
                        {//右上角和左下角值大于零
                            node.type = 6;
                            node.power = rtv;
                            node.stormID = timeImg[i, j + 1];//右上角栅格值
                        }
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                }
            }
            Node[] nodeArr = nodes.ToArray();
            #endregion
            #region 所有线
            List<Line> lines = new List<Line>();
            //结点连成线
            for (int i = 0; i < nodeArr.Length; i++)
            {//寻找每一条线
                if (nodeArr[i].isUsed == true || nodeArr[i].type > 4) continue;//被使用，跳出本次循环
                Node headNode = nodeArr[i];//头结点
                headNode.isUsed = true;//记录被使用
                headNode.outDir = headNode.dir1;//记录出去的方向
                nodeArr[i] = headNode;//进行保存
                Line line = new Line();//新建一条线
                int minRow = headNode.row;
                int minCol = headNode.col;
                int maxRow = headNode.row;
                int maxCol = headNode.col;
                //line.nodes.Add(headNode);//
                List<Node> lineNodes = new List<Node>();//线中所有点
                lineNodes.Add(headNode);//将第一个点添加进去
                Node tailNode = headNode;//尾结点
                //Node nextNode = new Node();//用来记录找到的下一个结点
                double lineLength = 0.0;//线的长度
                do
                {
                    int startNodeRow = tailNode.row;
                    int startNodeCol = tailNode.col;
                    tailNode = GetNextNode(tailNode, ref nodeArr, ref minRow, ref minCol, ref maxRow, ref maxCol);

                    //计算周长
                    double logInterval = Math.Abs(tailNode.col - startNodeCol) * resolution;//经度差值
                    if (logInterval == 0.0)
                    {//竖线
                        double latInterval = Math.Abs(tailNode.row - startNodeRow) * resolution;//纬度差值
                        double _length = Earth.ML * 0.001 * latInterval / 180.0;//经线长度为20017km（百度百科），每条经线长度都相同
                        lineLength += _length;
                    }
                    else
                    {//横线
                        double lineLat = startLat + (row - tailNode.row) * resolution;//纬度
                        double equatorLengthNow = Earth.EC * 0.001 * Math.Cos(lineLat * Math.PI / 180.0);//当前纬线周长
                        double _length = equatorLengthNow * logInterval / 360.0;
                        lineLength += _length;
                    }

                    lineNodes.Add(tailNode);//将点添加进去
                } while (tailNode.id != headNode.id);

                //判断是内环还是外环
                Node firstNode = lineNodes[0];//第一个结点
                if (firstNode.type == 1)
                {
                    //if (timeImg[firstNode.row, firstNode.col] > 0)//右下角栅格值
                    if (timeImg[firstNode.row, firstNode.col] != 0)
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 2)
                {
                    //if (timeImg[firstNode.row, firstNode.col + 1] > 0)//右下角栅格值
                    if (timeImg[firstNode.row, firstNode.col + 1] != 0)
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 3)
                {
                    //if (timeImg[firstNode.row + 1, firstNode.col] > 0)//右下角栅格值
                    if (timeImg[firstNode.row + 1, firstNode.col] != 0)
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }
                else if (firstNode.type == 4)
                {
                    //if (timeImg[firstNode.row + 1, firstNode.col + 1] > 0)//右下角栅格值
                    if (timeImg[firstNode.row + 1, firstNode.col + 1] != 0)
                    {
                        line.type = 0;//外环
                    }
                    else
                    {
                        line.type = 1;//内环
                    }
                }

                line.id = lines.Count;
                line.nodes = lineNodes;//进行保存
                line.minRow = minRow;
                line.minCol = minCol;
                line.maxRow = maxRow;
                line.maxCol = maxCol;
                line.power = headNode.power;//值
                line.length = lineLength;//线的长度
                //if (line.type == 1 && line.length < 1000) continue;//周长过小剔除
                lines.Add(line);
            }
            Line[] lineArr = lines.ToArray();
            #endregion
            #region 线构成面
            List<Polygon> polygons = new List<Polygon>();//所有面
            for (int i = 0; i < lineArr.Length; i++)
            {//循环每一条线
                if (lineArr[i].type != 0) continue;//不是外环，退出本次循环
                Polygon polygon = new Polygon();
                List<Line> pLines = new List<Line>();
                pLines.Add(lineArr[i]);//添加外环
                polygon.id = polygons.Count;
                polygon.minCol = lineArr[i].minCol;
                polygon.minRow = lineArr[i].minRow;
                polygon.maxCol = lineArr[i].maxCol;
                polygon.maxRow = lineArr[i].maxRow;
                polygon.power = lineArr[i].power;
                for (int j = i + 1; j < lineArr.Length; j++)
                {//寻找该外环包含的内环，内环肯定在后面
                    //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
                    if (lineArr[j].type == 0) continue;//外环
                    if (lineArr[j].minRow > lineArr[i].minRow && lineArr[j].minCol > lineArr[i].minCol && lineArr[j].maxRow < lineArr[i].maxRow && lineArr[j].maxCol < lineArr[i].maxCol)
                    {//邻接矩形包含，进一步判断
                        //只要一个点在外环内，就是内环
                        bool isIn = IsInPolygonNew(lineArr[j].nodes[0], lineArr[i].nodes);//判断是否在外环里面
                        if (isIn)
                        {//内环
                            Line line = lineArr[j];//取出线
                            line.nodes.Reverse();//顺序反转
                            //line.type = 1;
                            lineArr[j] = line;//保存修改后的
                            pLines.Add(line);//添加内环
                        }
                    }
                }
                polygon.lines = pLines;
                double volume = 0.0;//温度异常累加
                double intensityMean = 0.0;//平均温度异常
                double intensityMax = double.MinValue;//最大温度异常
                double intensityMin = double.MaxValue;//最小温度异常
                double uArea = 0.0;//经线方向速度与面积乘积
                double vArea = 0.0;//纬线方向速度与面积成绩
                //double avgRainfall = 0.0;//平均降雨量
                //double volume = 0.0;//累计降雨量
                //double maxRainfall = 0.0;//最大降雨量
                //double minRainfall = double.MaxValue;//最小降雨量
                double area = 0.0;//面积
                double _rowCore = 0.0;//重心行号中间量
                double _colCore = 0.0;//重心列号中间量
                double rasterArea = 0.0;
                for (int _row = polygon.minRow + 1; _row <= polygon.maxRow; _row++)
                {//行循环，行列号对应节点左上角栅格
                    double rasterStartLat = startLat + (row - _row - 1) * resolution;//栅格下边缘纬度
                    double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
                    rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积,单位平方km
                    for (int _col = polygon.minCol + 1; _col <= polygon.maxCol; _col++)
                    {//列循环
                        //if (idImg[_row, _col] == polygon.stormID)
                        if (IsInPolygonNew(_row - 0.5f, _col - 0.5f, pLines[0].nodes))
                        {//当前暴雨对象
                            float _rowF = _row - 0.5f;//栅格行列号转为矢量行列号
                            float _colF = _col - 0.5f;
                            if (IsInPolygonNew(_rowF, _colF, polygon.lines[0].nodes))
                            {//包含关系
                                double uSpeed = uImg[_row, _col] * uSacle;//流场数据单位为m/s
                                double vSpeed = vImg[_row, _col] * vSacle;
                                uArea += uSpeed * rasterArea;
                                vArea += vSpeed * rasterArea;
                                area += rasterArea;//增加面积
                                //double rainfall = oriImg[_row, _col] / timeCell;//换算为每小时
                                double intensity = timeImg[_row, _col];//每半小时
                                if (intensity > intensityMax) intensityMax = intensity;//最大降雨量
                                if (intensity < intensityMin) intensityMin = intensity;
                                double _volume = rasterArea * intensity;//暴雨总降雨量累加
                                volume += _volume;
                                //加权
                                _rowCore += _volume * _row;
                                _colCore += _volume * _col;
                            }
                        }
                    }
                }
                //if (area <= 3 * rasterArea)
                //{
                //    continue;
                //}
                //平均
                polygon.coreRow = _rowCore / volume;
                polygon.coreCol = _colCore / volume;

                intensityMean = volume / area;//计算平均降雨量
                polygon.intensityMean = intensityMean * scale;//保存平均降雨量
                polygon.intensityMax = intensityMax * scale;//保存最大降雨量
                polygon.intensityMin = intensityMin * scale;//保存最大降雨量
                polygon.area = area;

                polygon.uSpeed = uArea / area;
                polygon.vSpeed = vArea / area;

                //volume = volume * 1000;//立方米
                //polygon.volume = volume;
                polygon.length = polygon.lines[0].length;//周长
                polygon.minRec = GetMinAreaRec(polygon);
                polygon.minOutCir = GetMinOutCir(polygon);
                polygon.maxInCir = GetMaxInCir(polygon, 0.5);
                polygons.Add(polygon);
            }
            #endregion
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = outPath;
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);
            #region 接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("EventID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStateID = new FieldDefn("StateID", FieldType.OFTString);
            oFieldStateID.SetWidth(20);
            olayer1.CreateField(oFieldStateID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
            oFieldName.SetWidth(20);
            olayer1.CreateField(oFieldName, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldTime.SetWidth(20);
            olayer1.CreateField(oFieldTime, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            oFieldLongTime.SetWidth(20);
            olayer1.CreateField(oFieldLongTime, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
            oFieldMinLog.SetWidth(20);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建area字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            olayer1.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("IMean", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            ////创建体积字段
            //FieldDefn oFieldVolume = new FieldDefn("Volume", FieldType.OFTReal);
            //oFieldVolume.SetWidth(20);
            //oFieldVolume.SetPrecision(8);
            //olayer1.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("IMax", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMaxRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("IMin", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMinRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldUSpeed = new FieldDefn("USpeed", FieldType.OFTReal);
            oFieldUSpeed.SetWidth(20);
            oFieldUSpeed.SetPrecision(8);
            olayer1.CreateField(oFieldUSpeed, 1);

            //创建最大降雨量字段
            FieldDefn oFieldVSpeed = new FieldDefn("VSpeed", FieldType.OFTReal);
            oFieldVSpeed.SetWidth(20);
            oFieldVSpeed.SetPrecision(8);
            olayer1.CreateField(oFieldVSpeed, 1);
            
            //创建周长字段
            FieldDefn oFieldLength = new FieldDefn("Length", FieldType.OFTReal);
            oFieldLength.SetWidth(20);
            oFieldLength.SetPrecision(8);
            olayer1.CreateField(oFieldLength, 1);

            //创建质心字段
            FieldDefn oFieldLogCore = new FieldDefn("CoreLog", FieldType.OFTReal);
            oFieldLogCore.SetWidth(20);
            oFieldLogCore.SetPrecision(8);
            olayer1.CreateField(oFieldLogCore, 1);

            //创建质心字段
            FieldDefn oFieldLatCore = new FieldDefn("CoreLat", FieldType.OFTReal);
            oFieldLatCore.SetWidth(20);
            oFieldLatCore.SetPrecision(8);
            olayer1.CreateField(oFieldLatCore, 1);

            //创建形状系数字段
            FieldDefn oFieldSI = new FieldDefn("SI", FieldType.OFTReal);
            oFieldSI.SetWidth(20);
            oFieldSI.SetPrecision(8);
            olayer1.CreateField(oFieldSI, 1);

            //创建最大长度字段
            FieldDefn oFieldLMax = new FieldDefn("LMax", FieldType.OFTReal);
            oFieldLMax.SetWidth(20);
            oFieldLMax.SetPrecision(8);
            olayer1.CreateField(oFieldLMax, 1);

            //创建最大宽度字段
            FieldDefn oFieldWMax = new FieldDefn("WMax", FieldType.OFTReal);
            oFieldWMax.SetWidth(20);
            oFieldWMax.SetPrecision(8);
            olayer1.CreateField(oFieldWMax, 1);

            //创建偏心率字段
            FieldDefn oFieldERatio = new FieldDefn("ERatio", FieldType.OFTReal);
            oFieldERatio.SetWidth(20);
            oFieldERatio.SetPrecision(8);
            olayer1.CreateField(oFieldERatio, 1);

            //创建矩形度字段
            FieldDefn oFieldRecDeg = new FieldDefn("RecDeg", FieldType.OFTReal);
            oFieldRecDeg.SetWidth(20);
            oFieldRecDeg.SetPrecision(8);
            olayer1.CreateField(oFieldRecDeg, 1);

            //创建圆形度字段
            FieldDefn oFieldSphDeg = new FieldDefn("SphDeg", FieldType.OFTReal);
            oFieldSphDeg.SetWidth(20);
            oFieldSphDeg.SetPrecision(8);
            olayer1.CreateField(oFieldSphDeg, 1);

            //创建最小外接矩形点1
            FieldDefn oFieldMinRecP1X = new FieldDefn("RecP1X", FieldType.OFTString);
            oFieldMinRecP1X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP1X, 1);

            //创建最小外接矩形点1
            FieldDefn oFieldMinRecP1Y = new FieldDefn("RecP1Y", FieldType.OFTString);
            oFieldMinRecP1Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP1Y, 1);

            //创建最小外接矩形点2
            FieldDefn oFieldMinRecP2X = new FieldDefn("RecP2X", FieldType.OFTString);
            oFieldMinRecP2X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP2X, 1);

            //创建最小外接矩形点2
            FieldDefn oFieldMinRecP2Y = new FieldDefn("RecP2Y", FieldType.OFTString);
            oFieldMinRecP2Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP2Y, 1);

            //创建最小外接矩形点3
            FieldDefn oFieldMinRecP3X = new FieldDefn("RecP3X", FieldType.OFTString);
            oFieldMinRecP3X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP3X, 1);

            //创建最小外接矩形点3
            FieldDefn oFieldMinRecP3Y = new FieldDefn("RecP3Y", FieldType.OFTString);
            oFieldMinRecP3Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP3Y, 1);

            //创建最小外接矩形点4
            FieldDefn oFieldMinRecP4X = new FieldDefn("RecP4X", FieldType.OFTString);
            oFieldMinRecP4X.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP4X, 1);

            //创建最小外接矩形点4
            FieldDefn oFieldMinRecP4Y = new FieldDefn("RecP4Y", FieldType.OFTString);
            oFieldMinRecP4Y.SetWidth(20);
            olayer1.CreateField(oFieldMinRecP4Y, 1);

            //创建Power字段
            FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTInteger);
            olayer1.CreateField(oFieldPower, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP1 = new FieldDefn("P1", FieldType.OFTReal);
            //oFieldP1.SetWidth(10);
            //oFieldP1.SetPrecision(8);
            //olayer1.CreateField(oFieldP1, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP2 = new FieldDefn("P2", FieldType.OFTReal);
            //oFieldP2.SetWidth(10);
            //oFieldP2.SetPrecision(8);
            //olayer1.CreateField(oFieldP2, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP3 = new FieldDefn("P3", FieldType.OFTReal);
            //oFieldP3.SetWidth(10);
            //oFieldP3.SetPrecision(8);
            //olayer1.CreateField(oFieldP3, 1);

            ////创建形状系数字段
            //FieldDefn oFieldP4 = new FieldDefn("P4", FieldType.OFTReal);
            //oFieldP4.SetWidth(10);
            //oFieldP4.SetPrecision(8);
            //olayer1.CreateField(oFieldP4, 1);
            #endregion

            //写入数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 20000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, polygon.id);
                //oFeature.SetField(1, polygon.stormID);
                oFeature.SetField(3, "Sea Surface Temperature");

                oFeature.SetField(4, startTime.ToString());
                DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
                long timeStamp = (long)(startTime - time1970).TotalSeconds; // 相差秒数
                oFeature.SetField(5, timeStamp.ToString());

                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //计算经纬度
                double CoreLog = startLog + (polygon.coreCol + 0.5) * resolution;//质心经度
                double CoreLat = startLat + (row - polygon.coreRow - 0.5) * resolution;//质心纬度

                //	形状系数（SI）：面积（A）/周长（P）
                double si = (4 * Math.Sqrt(polygon.area)) / polygon.length;
                double eRatio = polygon.minRec.width / polygon.minRec.length;
                double recDeg = polygon.area / ((polygon.minRec.length * polygon.minRec.width) * 123.93 * Math.Cos((maxLat + minLat) / 2 * Math.PI / 180));//最小外包矩形面积为近似计算
                double sphDeg = polygon.maxInCir.r / polygon.minOutCir.r;

                oFeature.SetField(6, minLog);
                oFeature.SetField(7, minLat);
                oFeature.SetField(8, maxLog);
                oFeature.SetField(9, maxLat);

                oFeature.SetField(10, polygon.area);
                oFeature.SetField(11, polygon.intensityMean);
                //oFeature.SetField(12, polygon.volume);
                oFeature.SetField(12, polygon.intensityMax);
                oFeature.SetField(13, polygon.intensityMin);
                oFeature.SetField(14, polygon.uSpeed);
                oFeature.SetField(15, polygon.vSpeed);
                
                oFeature.SetField(16, polygon.length);
                oFeature.SetField(17, CoreLog);
                oFeature.SetField(18, CoreLat);
                oFeature.SetField(19, si);
                oFeature.SetField(20, polygon.minRec.length * resolution);
                oFeature.SetField(21, polygon.minRec.width * resolution);
                oFeature.SetField(22, eRatio);
                oFeature.SetField(23, recDeg);
                oFeature.SetField(24, sphDeg);

                double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
                double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
                double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
                double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

                oFeature.SetField(25, p1t[0]);
                oFeature.SetField(26, p1t[1]);
                oFeature.SetField(27, p2t[0]);
                oFeature.SetField(28, p2t[1]);
                oFeature.SetField(29, p3t[0]);
                oFeature.SetField(30, p3t[1]);
                oFeature.SetField(31, p4t[0]);
                oFeature.SetField(32, p4t[1]);

                oFeature.SetField(33, polygon.power);

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "POLYGON (";
                foreach (Line line in polygon.lines)
                {//写出多边形中每条线
                    polygonStr += "(";
                    foreach (Node node in line.nodes)
                    {
                        int _row = node.row;//点行号
                        int _col = node.col;//点列号

                        double log = startLog + (_col + 1) * resolution;//经度
                        double lat = startLat + (row - _row - 1) * resolution;//纬度

                        polygonStr += log.ToString() + " " + lat.ToString();
                        polygonStr += ",";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer1.Dispose();
            ds1.Dispose();

            TimeSpan ts = DateTime.Now - time1;
            Console.WriteLine(ts.TotalSeconds);

            #region 最小面积外包矩形输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile2 = Path.GetDirectoryName(outPath)+"\\1\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinAreaRec.shp";
            Ogr.RegisterAll();

            string strDriver2 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver2 = Ogr.GetDriverByName(strDriver2);
            if (oDriver2 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds2 = oDriver2.CreateDataSource(strVectorFile2, null);
            if (ds2 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt2 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr2 = new OSGeo.OSR.SpatialReference(wkt2);
            Layer olayer2 = ds2.CreateLayer("PolygonLayer", sr2, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn2 = olayer2.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn2);
                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                double[] p1t = { startLog + (polygon.minRec.p1[1] + 1) * resolution, startLat + (row - polygon.minRec.p1[0] - 1) * resolution };
                double[] p2t = { startLog + (polygon.minRec.p2[1] + 1) * resolution, startLat + (row - polygon.minRec.p2[0] - 1) * resolution };
                double[] p3t = { startLog + (polygon.minRec.p3[1] + 1) * resolution, startLat + (row - polygon.minRec.p3[0] - 1) * resolution };
                double[] p4t = { startLog + (polygon.minRec.p4[1] + 1) * resolution, startLat + (row - polygon.minRec.p4[0] - 1) * resolution };

                string polygonStr = "POLYGON (("+ p1t[0] + " " + p1t[1] + "," + p2t[0] + " " + p2t[1] + "," + p3t[0] + " " + p3t[1] + "," + p4t[0] + " " + p4t[1] + "," + p1t[0] + " " + p1t[1]+"))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer2.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer2.Dispose();
            ds2.Dispose();
            */
            #endregion

            #region 最小面积外接圆输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile3 = Path.GetDirectoryName(outPath)+"\\2\\" + Path.GetFileNameWithoutExtension(outPath)+"_MinOutCir.shp";
            Ogr.RegisterAll();

            string strDriver3 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver3 = Ogr.GetDriverByName(strDriver3);
            if (oDriver3 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds3 = oDriver3.CreateDataSource(strVectorFile3, null);
            if (ds3 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt3 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr3 = new OSGeo.OSR.SpatialReference(wkt3);
            Layer olayer3 = ds3.CreateLayer("PolygonLayer", sr3, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn3 = olayer3.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn3);
                //计算经纬度
                //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

                string polygonStr = "POLYGON ((";
                double cx = polygon.minOutCir.x;
                double cy = polygon.minOutCir.y;
                double cr = polygon.minOutCir.r;
                for (int i=0;i<=360;i++)
                {
                    double de = i * Math.PI / 180;//弧度制
                    double x = cx + cr*Math.Sin(de);
                    double y = cy + cr * Math.Cos(de);

                    double log= startLog + (y + 1) * resolution;//经度
                    double lat = startLat + (row - x - 1) * resolution;//纬度

                    polygonStr += log + " " + lat + ",";
                }


                polygonStr=polygonStr.TrimEnd(',');
                polygonStr+= "))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer3.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer3.Dispose();
            ds3.Dispose();
            */
            #endregion

            #region 最大面积内切圆输出shp(测试效果用)
            /*
            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile4 = Path.GetDirectoryName(outPath) + "\\3\\" + Path.GetFileNameWithoutExtension(outPath) + "_MaxInCir.shp";
            Ogr.RegisterAll();

            string strDriver4 = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver4 = Ogr.GetDriverByName(strDriver4);
            if (oDriver4 == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds4 = oDriver4.CreateDataSource(strVectorFile4, null);
            if (ds4 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt4 = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr4 = new OSGeo.OSR.SpatialReference(wkt4);
            Layer olayer4 = ds4.CreateLayer("PolygonLayer", sr4, wkbGeometryType.wkbPolygon, null);


            //写入数据
            FeatureDefn oDefn4 = olayer4.GetLayerDefn();

            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.area < 10000) continue;//面积太小的不要
                Feature oFeature = new Feature(oDefn4);
                //计算经纬度
                //double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                //double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                //double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                //double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                //oFeature.SetField(19, polygon.minRec.p1[0]+","+ polygon.minRec.p1[1]);
                //oFeature.SetField(20, polygon.minRec.p2[0] + "," + polygon.minRec.p2[1]);
                //oFeature.SetField(21, polygon.minRec.p3[0] + "," + polygon.minRec.p3[1]);
                //oFeature.SetField(22, polygon.minRec.p4[0] + "," + polygon.minRec.p4[1]);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"

                string polygonStr = "POLYGON ((";
                double cx = polygon.maxInCir.x;
                double cy = polygon.maxInCir.y;
                double cr = polygon.maxInCir.r;
                for (int i = 0; i <= 360; i++)
                {
                    double de = i * Math.PI / 180;//弧度制
                    double x = cx + cr * Math.Sin(de);
                    double y = cy + cr * Math.Cos(de);

                    double log = startLog + (y + 1) * resolution;//经度
                    double lat = startLat + (row - x - 1) * resolution;//纬度

                    polygonStr += log + " " + lat + ",";
                }


                polygonStr = polygonStr.TrimEnd(',');
                polygonStr += "))";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer4.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer4.Dispose();
            ds4.Dispose();
            */
            #endregion
        }


        #region 计算最小面积外包矩形
        private static Rectangle GetMinAreaRec(Polygon polygon)
        {
            Rectangle minRec = new Rectangle(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, double.MaxValue);

            Line line1 = polygon.lines[0];
            List<Node> pointList = line1.nodes;

            int[,] points = getPoints(pointList);//row=x col=y

            int minAreaAngle = 0;//最小外包矩形旋转角度

            for (int angle = 0; angle < 360; angle++)
            {//旋转每个角度
                double[,] pointsR = getPointsRotate(points, angle);//获取旋转后的点
                Rectangle rec = getRec(pointsR);//计算平行于坐标轴的最小外包矩形
                if (rec.area < minRec.area)
                {//更小的外包矩形
                    minRec = rec;
                    minAreaAngle = angle;
                }
            }
            minRec.area = (minRec.maxX - minRec.minX) * (minRec.maxY - minRec.minY);
            Rectangle minRecR = getRecRotate(minRec, -minAreaAngle);//获取旋转后的矩形

            return minRecR;
        }

        private static Rectangle getRecRotate(Rectangle minRec, int angle)
        {
            double[] p1 = { minRec.minX, minRec.minY };
            double[] p2 = { minRec.maxX, minRec.minY };
            double[] p3 = { minRec.maxX, minRec.maxY };
            double[] p4 = { minRec.minX, minRec.maxY };
            double[] p1R = GetPointRotate(p1[0], p1[1], angle);
            double[] p2R = GetPointRotate(p2[0], p2[1], angle);
            double[] p3R = GetPointRotate(p3[0], p3[1], angle);
            double[] p4R = GetPointRotate(p4[0], p4[1], angle);

            double length = minRec.maxX - minRec.minX;
            double width = minRec.maxY - minRec.minY;
            if(width>length)
            {//交换长宽
                double temp = length;
                length = width;
                width = temp;
            }

            Rectangle RecR = new Rectangle(p1R, p2R, p3R, p4R, length, width, minRec.area);
            return RecR;
        }

        private static Rectangle getRec(double[,] pointsR)
        {
            Rectangle rec = new Rectangle();
            rec.minX = pointsR[0, 0];
            rec.maxX = pointsR[0, 0];
            rec.minY = pointsR[0, 1];
            rec.maxY = pointsR[0, 1];
            for (int i = 0; i < pointsR.GetLength(0); i++)
            {//每个点
                double _x = pointsR[i, 0];
                double _y = pointsR[i, 1];
                if (_x < rec.minX) rec.minX = _x; else if (_x > rec.maxX) rec.maxX = _x;
                if (_y < rec.minY) rec.minY = _y; else if (_y > rec.maxY) rec.maxY = _y;
            }
            rec.area = (rec.maxX - rec.minX) * (rec.maxY - rec.minY);
            return rec;
        }

        private static double[,] getPointsRotate(int[,] points, int angle)
        {
            double[,] pointsR = new double[points.GetLength(0), points.GetLength(1)];//用来存储旋转后的点
            for (int i = 0; i < points.GetLength(0); i++)
            {
                int[] point = { points[i, 0], points[i, 1] };//取出一个点
                double[] pointR = GetPointRotate(point[0], point[1], angle);//获取改点绕原点旋转坐标
                pointsR[i, 0] = pointR[0];
                pointsR[i, 1] = pointR[1];
            }
            return pointsR;
        }

        private static double[] GetPointRotate(double x, double y, int angle)
        {
            double angleR = angle * Math.PI / 180;//计算弧度制角度
            double xr = x * Math.Cos(angleR) + y * Math.Sin(angleR);
            double yr = -x * Math.Sin(angleR) + y * Math.Cos(angleR);
            double[] pr = { xr, yr };
            return pr;
        }

        private static int[,] getPoints(List<Node> pointList)
        {
            int[,] points = new int[pointList.Count, 2];

            for (int i = 0; i < pointList.Count; i++)
            {
                points[i, 0] = pointList[i].row;
                points[i, 1] = pointList[i].col;
            }

            return points;
        }
        #endregion

        #region 最大面积内切圆
        /// <summary>
        /// 求最大面积内切圆（遗传算法）
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private static Circle GetMaxInCir(Polygon polygon,double step)
        {
            double minX = polygon.minRow;
            double minY = polygon.minCol;
            double maxX = polygon.maxRow;
            double maxY = polygon.maxCol;

            //double[,] ccs = new double[Convert.ToInt32(((maxX - minX) / step - 1) * ((maxY - minY) / step - 1)), 2];//初始远心点
            List<Circle> cirs = new List<Circle>();//最大半径的圆
            cirs.Add(new Circle());
            //double radiusMax = 0.0;
            //double xm = 0.0;
            //double ym = 0.0;
            for (double _x=minX+ step;_x<maxX;_x+= step)
            {
                for(double _y=minY+step;_y<maxY;_y+=step)
                {
                    if (IsInPolygonNoBorder(_x, _y, polygon.lines[0].nodes))
                    {//点位于多边形内
                        double radius = GetRadiusPoint(polygon, _x, _y);//获取该点为圆心的内切圆半径

                        if(radius> cirs[0].r)
                        {
                            cirs.Clear();
                            cirs.Add(new Circle(_x, _y, radius));
                        }
                        else if(radius == cirs[0].r)
                        {
                            cirs.Add(new Circle(_x, _y, radius));
                        }

                        //bool isInsert = false;
                        //for (int i = 0; i < cirs.Count; i++)
                        //{
                        //    if (cirs[i].r < radius)
                        //    {
                        //        cirs.Insert(i, new Circle(_x, _y, radius));
                        //        if (cirs.Count > 3) cirs.RemoveAt(cirs.Count - 1);
                        //        isInsert = true;
                        //        break;
                        //    }
                        //}
                        //if(!isInsert&&cirs.Count<3) cirs.Add(new Circle(_x, _y, radius));

                        //if(radius> radiusMax)
                        //{//更大的半径
                        //    radiusMax = radius;
                        //    xm = _x;
                        //    ym = _y;
                        //}
                    }
                }
            }
            Circle maxInCir = new Circle();
            foreach (Circle cir in cirs)
            {
                Circle _maxInCir = GetMaxInCir(polygon, step, cir.x, cir.y);//计算调整后的圆
                if (_maxInCir.r > maxInCir.r) maxInCir = _maxInCir;//寻找最大半径圆
            }
            return maxInCir;
        }

        /// <summary>
        /// 求最大面积内切圆（遗传算法）,优化前，耗时巨大
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private static Circle GetMaxInCirBefor(Polygon polygon, double step)
        {
            double minX = polygon.minRow;
            double minY = polygon.minCol;
            double maxX = polygon.maxRow;
            double maxY = polygon.maxCol;

            //double[,] ccs = new double[Convert.ToInt32(((maxX - minX) / step - 1) * ((maxY - minY) / step - 1)), 2];//初始远心点
            Circle maxInCir = new Circle();//用来记录最大面积内切圆
            for (double _x = minX + step; _x < maxX; _x += step)
            {
                for (double _y = minY + step; _y < maxY; _y += step)
                {
                    if (IsInPolygonNoBorder(_x, _y, polygon.lines[0].nodes))
                    {//点位于多边形内
                        Circle cir = GetMaxInCir(polygon, step, _x, _y);//计算调整后的圆
                        if (cir.r > maxInCir.r) maxInCir = cir;//半径（面积）更大的圆
                    }
                }
            }
            return maxInCir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        private static double GetRadiusPoint(Polygon polygon, double x, double y)
        {
            Line line = polygon.lines[0];
            List<Node> nodeList = line.nodes;
            double[,] points = new double[nodeList.Count, 2];
            for (int i = 0; i < nodeList.Count; i++)
            {//取出xy
                points[i, 0] = nodeList[i].row;//x
                points[i, 1] = nodeList[i].col;//y
            }

            double distances = double.MaxValue;//点与线段的距离
            for (int i = 0; i < nodeList.Count - 1; i++)
            {//计算距离
                double _distances = GetPointLineDistance(x, y, points[i, 0], points[i, 1], points[i + 1, 0], points[i + 1, 1]);//计算距离
                if (_distances < distances) distances = _distances;//记录更小的
            }

            return distances;
        }

        /// <summary>
        /// 寻找面积最大的内切圆
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="step"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static Circle GetMaxInCir(Polygon polygon, double step, double x, double y)
        {
            double xOir = x;
            double yOir = y;
            if(xOir== 561&& yOir== 639.5)
            {
                int aa = 0;
            }
            double stepOir = step;//记录原始值
            step = step * 0.5;
            Line line = polygon.lines[0];
            List<Node> nodeList = line.nodes;
            double[,] points = new double[nodeList.Count, 2];
            for(int i=0;i<nodeList.Count;i++)
            {//取出xy
                points[i, 0] = nodeList[i].row;//x
                points[i, 1] = nodeList[i].col;//y
            }

            double lastStandDev = double.MaxValue;//记录上一个标准差
            double standDev = double.MaxValue;//当前标准差
            double r = 0.0;//圆的半径
            while(true)
            {//迭代求最大面积内切圆
                double[] distances = new double[nodeList.Count - 1];//点与线段的距离
                double[,] disPoints = new double[nodeList.Count - 1,2];//计算线段距离的另一个端点

                List<int> minDisPosList = new List<int>();//记录距离最小点位置
                for (int i = 0; i < distances.Length; i++)
                {//计算距离
                    double[] disPoint = new double[2];//计算距离的另一个点
                    distances[i] = Math.Round(GetPointLineDistance(x, y, points[i, 0], points[i, 1], points[i + 1, 0], points[i + 1, 1],ref disPoint),4);//计算距离
                    disPoints[i, 0] = disPoint[0];//记录计算距离端点位置
                    disPoints[i, 1] = disPoint[1];
                    bool isInsert = false;//记录是否插入
                    for (int j = 0; j < minDisPosList.Count; j++)
                    {
                        if (distances[i] < distances[minDisPosList[j]])
                        {//更小的距离
                            minDisPosList.Insert(j, i);//插入该位置
                            if (minDisPosList.Count > 3) minDisPosList.RemoveAt(minDisPosList.Count-1);//移除后面的
                            isInsert = true;
                            break;
                        }
                    }
                    if(!isInsert&& minDisPosList.Count<3)
                    {//没有插入
                        minDisPosList.Add(i);//在最后记录下该位置
                    }
                    //if (minDisPosList.Count > 3) minDisPosList.RemoveRange(3, minDisPosList.Count() - 3);//移除后面的
                }
                r = distances.Min();//选取最小距离最为圆的半径
                //if(r>1000)
                //{
                //    r = 0;
                //}
                standDev = GetStandDev(distances[minDisPosList[0]], distances[minDisPosList[1]], distances[minDisPosList[2]]);//计算标准

                if (standDev < (stepOir * 0.01))
                {//标准差很小
                    break;//结束迭代运算
                }
                else
                {
                    int minDisPos = minDisPosList[0];//最短距离位置
                    double[] pointMove = GetMovePoint(disPoints[minDisPos, 0], disPoints[minDisPos, 1], x, y, step);//获取移动后的点
                    x = pointMove[0];
                    y = pointMove[1];
                    double[] pointMove2 = GetMovePoint(disPoints[minDisPos, 0], disPoints[minDisPos, 1], 561, 639.5, step);//获取移动后的点
                }

                if (standDev >= lastStandDev)
                {//标准差变大了

                    step = step * 0.5;//调整距离变为减半
                }
                lastStandDev = standDev;//记录标准差

                if (step < (stepOir * (Math.Pow(0.5,100))))
                {//调整很小
                    break;//结束迭代运算
                }
            }
            Circle cir = new Circle(x, y, r);
            return cir;
        }

        private static double[] GetMovePoint(double x1, double y1, double x, double y, double step)
        {
            double length = Math.Sqrt(Math.Pow((x1 - x), 2) + Math.Pow((y1 - y), 2));//线段长度
            double xm =x+ ((x - x1) / length) * step;//计算移动后的x坐标
            double ym =y+ ((y - y1) / length) * step;//计算移动后的y坐标
            double[] mp = { xm, ym };
            return mp;
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        private static double GetStandDev(double v1, double v2, double v3)
        {
            double avg = (v1 + v2 + v3) / 3.0;//平均值
            double variance = (Math.Pow(v1 - avg, 2) + Math.Pow(v2 - avg, 2) + Math.Pow(v3 - avg, 2)) / 3;//方差
            double dev = Math.Sqrt(variance);//标准差
            return dev;
        }

        private static double GetPointLineDistance(double x, double y, double px, double py, double nx, double ny,ref double[] disPoint)
        {
            double distance = 0.0;
            if(px==nx)
            {//竖线
                if(y>Max(py,ny)||y<Min(py,ny))
                {//垂足不在线段上
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if(d1<d2)
                    {
                        disPoint[0] = px;
                        disPoint[1] = py;
                        distance = d1;
                    }
                    else
                    {
                        disPoint[0] = nx;
                        disPoint[1] = ny;
                        distance = d2;
                    }
                }
                else
                {
                    disPoint[0] = px;
                    disPoint[1] = y;
                    distance = Math.Abs(x - px);
                }
            }
            else if(py==ny)
            {//横线
                if (x > Max(px, nx) || x < Min(px, nx))
                {
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        disPoint[0] = px;
                        disPoint[1] = py;
                        distance = d1;
                    }
                    else
                    {
                        disPoint[0] = nx;
                        disPoint[1] = ny;
                        distance = d2;
                    }
                }
                else
                {
                    disPoint[0] = x;
                    disPoint[1] = py;
                    distance = Math.Abs(y - py);
                }
            }
            else
            {
                Console.WriteLine("没有考虑该情况哦！");
            }
            return distance;
        }

        private static double GetPointLineDistance(double x, double y, double px, double py, double nx, double ny)
        {
            double distance = 0.0;
            if (px == nx)
            {//竖线
                if (y > Max(py, ny) || y < Min(py, ny))
                {//垂足不在线段上
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        distance = d1;
                    }
                    else
                    {
                        distance = d2;
                    }
                }
                else
                {
                    distance = Math.Abs(x - px);
                }
            }
            else if (py == ny)
            {//横线
                if (x > Max(px, nx) || x < Min(px, nx))
                {
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        distance = d1;
                    }
                    else
                    {
                        distance = d2;
                    }
                }
                else
                {
                    distance = Math.Abs(y - py);
                }
            }
            else
            {
                Console.WriteLine("没有考虑该情况哦！");
            }
            return distance;
        }

        /// <summary>
        /// 计算两点间的距离
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private static double GetPointDistance(double x1, double y1, double x2, double y2)
        {
            double dis = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return dis;
        }

        /// <summary>
        /// 返回两者较小值
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static double Min(double v1, double v2)
        {
            if (v1 < v2) return v1;
            else return v2;
        }

        /// <summary>
        /// 返回两者较大值
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private static double Max(double v1, double v2)
        {
            if (v1 > v2) return v1;
            else return v2;
        }
        #endregion

        #region 最小面积外接圆
        private static Circle GetMinOutCir(Polygon polygon)
        { 
            Circle minCircle = new Circle();

            Line line1 = polygon.lines[0];
            List<Node> pointList = line1.nodes;

            int[,] points = getPoints(pointList);//row=x col=y

            int minAngle = 0;//符合条件时旋转角度

            for (int angle = 0; angle < 360; angle++)
            {//旋转每个角度
                double[,] pointsR = getPointsRotate(points, angle);//获取旋转后的点
                Circle cir = getVerCir(pointsR);//计算以垂直方向最大最小点确定的面积最小的点
                if (cir.r >minCircle.r)
                {//更大半径的外接圆
                    minCircle = cir;
                    minAngle = angle;
                }
            }

            Circle minRecR = getCircleRotate(minCircle, -minAngle);//获取旋转后的矩形

            return minRecR;
        }

        private static Circle getCircleRotate(Circle minCircle, int angle)
        {
            double[] pointR = GetPointRotate(minCircle.x,minCircle.y, angle);
            Circle minCirR = new Circle(pointR[0], pointR[1], minCircle.r);
            return minCirR;
        }

        private static Circle getVerCir(double[,] pointsR)
        {
            double[] maxYPoint = { 0, double.MinValue };
            double[] minYPoint = { 0, double.MaxValue };
            for(int i=0;i< pointsR.GetLength(0);i++)
            {
                double[] point = { pointsR[i,0], pointsR[i,1] };//取出该点
                if (point[1] > maxYPoint[1]) maxYPoint = point;//记录y值更大的点
                if (point[1] < minYPoint[1]) minYPoint = point;//记录y值更小的点
            }
            double x = (maxYPoint[0] + minYPoint[0]) / 2;//圆心横坐标
            double y = (maxYPoint[1] + minYPoint[1]) / 2;//圆心纵坐标
            double r = (maxYPoint[1] - minYPoint[1]) / 2;//圆心半径
            Circle cir = new Circle(x, y, r);
            return cir;
        }
        #endregion

        /// <summary>
        /// 新版本,多面（未完成，弃用）
        /// </summary>
        /// <param name="oriPath"></param>
        /// <param name="spPath"></param>
        /// <param name="idPath"></param>
        /// <param name="outPath"></param>
        public static void TifToShpMulti(string oriPath, double valueScale,double timeCell, string spPath, string idPath, string outPath)
        {
            //string inPath = @"E:\strom\space\20170601-S003000-E005959Precipitation_Resample_Time_Spatial.tif";//输入路径
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

            //打开hdf文件
            Dataset oriDs = Gdal.Open(oriPath, Access.GA_ReadOnly);
            Dataset spDs = Gdal.Open(spPath, Access.GA_ReadOnly);
            Dataset idDs = Gdal.Open(idPath, Access.GA_ReadOnly);
            int col = oriDs.RasterXSize;//列数
            int row = oriDs.RasterYSize;//行数
            Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
            Band spBand1 = spDs.GetRasterBand(1);//读取波段
            Band idBand1 = idDs.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            oriDs.GetGeoTransform(argout);//读取地理坐标信息

            string[] metadatas = oriDs.GetMetadata("");//获取元数据
            double startLog = 70.0;//起始经度
            double startLat = 3.0;//起始维度
            double endLog = 140.0;//结束经度
            double endLat = 53.0;//结束维度
            double mScale = 1.0;//比例
            string dataType = "";
            string imgDate = "";
            string fillValue = "";
            double resolution = 0.1;//分辨率
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        mScale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "DataType":
                        dataType = mdArr[1];//起始经度
                        break;
                    case "ImageDate":
                        imgDate = mdArr[1];
                        break;
                    case "FillValue":
                        fillValue = mdArr[1];
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }

            double[] oriData = new double[row * col];//存储
            int[] spData = new int[row * col];//存储
            int[] idData = new int[row * col];//存储
            oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
            spBand1.ReadRaster(0, 0, col, row, spData, col, row, 0, 0);//读取数据
            idBand1.ReadRaster(0, 0, col, row, idData, col, row, 0, 0);//读取数据
            oriDs.Dispose();
            spDs.Dispose();
            idDs.Dispose();

            double[,] oriImg = new double[row, col];//二维数组，边缘不处理
            int[,] spImg = new int[row, col];//二维数组，边缘不处理
            int[,] idImg = new int[row, col];//二维数组，边缘不处理
            for (int i = 0; i < spData.Length; i++)
            {//将一维数组转换为二维，方便处理
                int _rowNow = i / col;//行号
                int _colNow = i % col;//列号
                if (_rowNow == 0 || _colNow == 0 || _rowNow == spImg.GetLength(0) - 1 || _colNow == spImg.GetLength(1) - 1) continue;//边界点不添加
                if(oriData[i]>0.0)
                {
                    oriImg[_rowNow, _colNow] = oriData[i] * valueScale * mScale;//乘以系数
                }
                else
                {//将0和负值赋值为0
                    oriImg[_rowNow, _colNow] = 0.0;
                }
                if(spData[i]>0)
                {
                    spImg[_rowNow, _colNow] = spData[i];
                }
                else
                {
                    spImg[_rowNow, _colNow] = 0;
                }
                if(idData[i]>0) idImg[_rowNow, _colNow] = idData[i];
                else idImg[_rowNow, _colNow] = 0;
            }


            //寻找结点
            List<Node> nodes = new List<Node>();//结点链表
            for (int i = 0; i < spImg.GetLength(0) - 1; i++)
            {//行循环
                for (int j = 0; j < spImg.GetLength(1) - 1; j++)
                {
                    int ltv = spImg[i, j];//左上角栅格值
                    int rtv = spImg[i, j + 1];//右上角栅格值
                    int lbv = spImg[i + 1, j];//左下角栅格值
                    int rbv = spImg[i + 1, j + 1];//右下角栅格值
                    if (ltv != rtv && rtv == lbv && lbv == rbv)
                    {//左上角不同，左和上方向
                        string dir1 = "l";//左方向
                        string dir2 = "t";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 1;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (ltv > 0)
                        {
                            node.power = ltv;
                            node.stormID= idImg[i, j];//左上角栅格值
                        }
                        else
                        {
                            node.power = rtv;
                            node.stormID = idImg[i, j + 1];//右上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (rtv != ltv && ltv == lbv && lbv == rbv)
                    {//右上角不同，上和右方向
                        string dir1 = "t";//左方向
                        string dir2 = "r";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 2;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rtv > 0)
                        {
                            node.power = rtv;
                            node.stormID = idImg[i, j + 1];//右上角栅格值
                        }
                        else
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (lbv != ltv && ltv == rtv && rtv == rbv)
                    {//左下角不同，下和左方向
                        string dir1 = "b";//左方向
                        string dir2 = "l";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 3;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (lbv > 0)
                        {
                            node.power = lbv;
                            node.stormID = idImg[i + 1, j];//左下角栅格值

                        }
                        else
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (rbv != ltv && ltv == rtv && rtv == lbv)
                    {//右下角不同，右和下方向
                        string dir1 = "r";//左方向
                        string dir2 = "b";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        node.type = 4;
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        if (rbv > 0)
                        {
                            node.power = rbv;
                            node.stormID = idImg[i + 1, j + 1];//右下角栅格值
                        }
                        else
                        {
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        nodes.Add(node);
                    }
                    else if (ltv == rbv && ltv != rtv && rtv == lbv)
                    {//对角相等，相邻不等
                        string dir1 = "n";//左方向
                        string dir2 = "n";//上方向
                        //double log = startLog + j * resolution;//经度
                        //double lat = endLat - i * resolution;//纬度
                        Node node = new Node();
                        node.id = nodes.Count;
                        if (ltv > 0)
                        {//左上角和右下角值大于零
                            node.type = 5;
                            node.power = ltv;
                            node.stormID = idImg[i, j];//左上角栅格值
                        }
                        else
                        {//右上角和左下角值大于零
                            node.type = 6;
                            node.power = rtv;
                            node.stormID = idImg[i, j + 1];//右上角栅格值
                        }
                        node.row = i;
                        node.col = j;
                        node.dir1 = dir1;
                        node.dir2 = dir2;
                        node.isUsed = false;
                        nodes.Add(node);
                    }
                }
            }

            List<Line> lines = new List<Line>();//所有线
            //结点连成线
            for (int i = 0; i < nodes.Count; i++)
            {//寻找每一条线
                if (nodes[i].isUsed == true || nodes[i].type > 4) continue;//被使用，跳出本次循环
                Node headNode = nodes[i];//头结点
                headNode.isUsed = true;//记录被使用
                headNode.outDir = headNode.dir1;//记录出去的方向
                nodes[i] = headNode;//进行保存
                Line line = new Line();//新建一条线
                int minRow = headNode.row;
                int minCol = headNode.col;
                int maxRow = headNode.row;
                int maxCol = headNode.col;
                //line.nodes.Add(headNode);//
                List<Node> lineNodes = new List<Node>();//线中所有点
                lineNodes.Add(headNode);//将第一个点添加进去
                Node tailNode = headNode;//尾结点
                //Node nextNode = new Node();//用来记录找到的下一个结点
                do
                {
                    tailNode = GetNextNode(tailNode, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                    lineNodes.Add(tailNode);//将点添加进去
                } while (tailNode.id != headNode.id);

                line.id = lines.Count;
                line.nodes = lineNodes;//进行保存
                line.minRow = minRow;
                line.minCol = minCol;
                line.maxRow = maxRow;
                line.maxCol = maxCol;
                line.type = 0;//默认外环
                line.power = headNode.power;//值
                line.stormID = headNode.stormID;//值
                lines.Add(line);
            }

            //线构成面
            List<Polygon> polygons = new List<Polygon>();//所有面
            for (int i = 0; i < lines.Count; i++)
            {//循环每一条线
                if (lines[i].type != 0) continue;//不是外环，退出本次循环
                Polygon polygon = new Polygon();
                List<Line> pLines = new List<Line>();
                pLines.Add(lines[i]);//添加外环
                polygon.id = polygons.Count;
                polygon.stormID = lines[i].stormID;
                polygon.minCol = lines[i].minCol;
                polygon.minRow = lines[i].minRow;
                polygon.maxCol = lines[i].maxCol;
                polygon.maxRow = lines[i].maxRow;
                polygon.power = lines[i].power;
                for (int j = i + 1; j < lines.Count; j++)
                {//寻找该外环包含的内环，内环肯定在后面
                    //if (lines[j].minRow >= lines[i].maxRow) break;//最小行数大于等于最大行数，不需要继续执行
                    if (lines[j].minRow > lines[i].minRow && lines[j].minCol > lines[i].minCol && lines[j].maxRow < lines[i].maxRow && lines[j].maxCol < lines[i].maxCol)
                    {//邻接矩形包含，进一步判断
                        //只要一个点在外环内，就是内环
                        bool isIn = IsInPolygonNew(lines[j].nodes[0], lines[i].nodes);//判断是否在外环里面
                        if (isIn)
                        {//内环
                            Line line = lines[j];//取出线
                            line.nodes.Reverse();//顺序反转
                            line.type = 1;
                            lines[j] = line;//保存修改后的
                            pLines.Add(line);//添加内环
                        }
                    }
                }
                polygon.lines = pLines;
                double avgRainfall = 0.0;//平均降雨量
                double sumRainfall = 0.0;//平均降雨量
                double area = 0.0;//面积
                for(int _row=polygon.minRow+1;_row<=polygon.maxRow;_row++)
                {//行循环，行列号对应节点左上角栅格
                    double rasterStartLat = startLat + (row - _row - 1) * resolution;//栅格下边缘纬度
                    double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
                    double rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积，平方千米
                    for(int _col=polygon.minCol+1;_col<=polygon.maxCol;_col++)
                    {//列循环
                        if(idImg[_row,_col]==polygon.stormID)
                        {//当前暴雨对象
                            area += rasterArea;//增加面积
                            double rainfall = oriImg[_row, _col];
                            if (rainfall < 0.0) rainfall = 0;
                            sumRainfall += rasterArea * oriImg[_row, _col];//暴雨总降雨量累加
                        }
                    }
                }
                avgRainfall = sumRainfall / area;//计算平均降雨量，毫米
                polygon.avgRainfall = avgRainfall;//保存平均降雨量
                polygon.area = area;

                sumRainfall = sumRainfall * 1000;//立方米
                polygon.volume = sumRainfall;
                polygon.isMulti = false;
                polygons.Add(polygon);
            }

            //构成多面
            List<MultiPolygon> multiPolygons = new List<MultiPolygon>();//多面
            for(int i=0;i<polygons.Count;i++)
            {
                if (polygons[i].isMulti) continue;//已经被处理为组合多边形
                Polygon fistPolygon = polygons[i];//第一个多边形
                MultiPolygon multiPolygon = new MultiPolygon();
                double area = fistPolygon.area;//总面积
                double sumRain = fistPolygon.volume;//总降雨量
                int minRow = fistPolygon.minRow;
                int minCol = fistPolygon.minCol;
                int maxRow = fistPolygon.maxRow;
                int maxCol = fistPolygon.maxCol;
                for (int j=i+1;j< polygons.Count;j++)
                {
                    if(polygons[j].stormID== fistPolygon.stormID)
                    {//一个暴雨事件
                        Polygon nextPolygon = polygons[j];//第一个多边形
                        nextPolygon.isMulti = true;//记录已经处理为组合多边形
                        polygons[j] = nextPolygon;//替换保存
                        area += nextPolygon.area;//面积
                        sumRain += nextPolygon.volume;//累计降雨量
                        if (nextPolygon.minRow < minRow) minRow = nextPolygon.minRow;//最小行数
                        if (nextPolygon.minCol < minCol) minCol = nextPolygon.minCol;//最小列数
                        if (nextPolygon.maxRow > maxRow) maxRow = nextPolygon.maxRow;//最大行数
                        if (nextPolygon.maxCol > maxCol) maxCol = nextPolygon.maxCol;//最大列数 
                        multiPolygon.polygons.Add(nextPolygon);//添加第一个多边形
                    }
                }
                if(multiPolygon.polygons.Count>1)
                {//组合面，保存
                    fistPolygon.isMulti = true;//记录是组合面
                    polygons[i] = fistPolygon;//保存
                    multiPolygon.id = fistPolygon.id;//唯一id
                    multiPolygon.stormID = fistPolygon.stormID;//暴雨事件id
                    multiPolygon.area = area;//平方千米
                    multiPolygon.volume = sumRain;//立方米
                    multiPolygon.avgRainfall = sumRain / (area * 1000);//平均降雨量，毫米
                    multiPolygon.power = fistPolygon.power;//降雨强度
                    multiPolygon.minRow = minRow;
                    multiPolygon.minCol = minCol;
                    multiPolygon.maxRow = maxRow;
                    multiPolygon.maxCol = maxCol;
                }
            }

            //保存shp
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = outPath;
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);
            //接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("ID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("StormID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("Name", FieldType.OFTString);
            oFieldName.SetWidth(10);
            olayer1.CreateField(oFieldName, 1);

            //创建x坐标字段
            FieldDefn oFieldValue = new FieldDefn("Power", FieldType.OFTReal);
            oFieldValue.SetWidth(10);
            oFieldValue.SetPrecision(8);
            olayer1.CreateField(oFieldValue, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLog", FieldType.OFTReal);
            oFieldMinLog.SetWidth(10);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(10);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLog", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(10);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(10);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建area坐标字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(10);
            oFieldArea.SetPrecision(8);
            olayer1.CreateField(oFieldArea, 1);

            //创建累计降雨量坐标字段
            FieldDefn oFieldSumRainFall = new FieldDefn("SumRain", FieldType.OFTReal);
            oFieldSumRainFall.SetWidth(20);
            oFieldSumRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldSumRainFall, 1);

            //创建平均降雨量坐标字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgRain", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            //写入数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();

            //写入单独面
            foreach (Polygon polygon in polygons)
            {//写出每个多边形
                if (polygon.isMulti) continue;//组合面不处理
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, polygon.id);
                oFeature.SetField(1, polygon.stormID);
                oFeature.SetField(2, "storm");
                oFeature.SetField(3, polygon.power);

                //计算经纬度
                double minLog = startLog + (polygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - polygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (polygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - polygon.minRow - 1) * resolution;//最大纬度

                oFeature.SetField(4, minLog);
                oFeature.SetField(5, minLat);
                oFeature.SetField(6, maxLog);
                oFeature.SetField(7, maxLat);

                oFeature.SetField(8, polygon.area);
                oFeature.SetField(9, polygon.volume);
                oFeature.SetField(10, polygon.avgRainfall);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "POLYGON (";
                foreach (Line line in polygon.lines)
                {//写出多边形中每条线
                    polygonStr += "(";
                    foreach (Node node in line.nodes)
                    {
                        int _row = node.row;//点行号
                        int _col = node.col;//点列号

                        double log = startLog + (_col + 1) * resolution;//经度
                        double lat = startLat + (row - _row - 1) * resolution;//纬度

                        polygonStr += log.ToString() + " " + lat.ToString();
                        polygonStr += ",";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }


            //写入组合面
            foreach (MultiPolygon multiPolygon in multiPolygons)
            {//写出每个组合多边形
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, multiPolygon.id);
                oFeature.SetField(1, multiPolygon.stormID);
                oFeature.SetField(2, "storm");
                oFeature.SetField(3, multiPolygon.power);

                //计算经纬度
                double minLog = startLog + (multiPolygon.minCol + 1) * resolution;//最小经度
                double minLat = startLat + (row - multiPolygon.maxRow - 1) * resolution;//最小纬度
                double maxLog = startLog + (multiPolygon.maxCol + 1) * resolution;//最大经度
                double maxLat = startLat + (row - multiPolygon.minRow - 1) * resolution;//最大纬度

                oFeature.SetField(4, minLog);
                oFeature.SetField(5, minLat);
                oFeature.SetField(6, maxLog);
                oFeature.SetField(7, maxLat);

                oFeature.SetField(8, multiPolygon.area);
                oFeature.SetField(9, multiPolygon.volume);
                oFeature.SetField(10, multiPolygon.avgRainfall);
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = "MULTIPOLYGON (";
                foreach(Polygon polygon in multiPolygon.polygons)
                {//每个多边形
                    polygonStr += "(";
                    foreach (Line line in polygon.lines)
                    {//写出多边形中每条线
                        polygonStr += "(";
                        foreach (Node node in line.nodes)
                        {
                            int _row = node.row;//点行号
                            int _col = node.col;//点列号

                            double log = startLog + (_col + 1) * resolution;//经度
                            double lat = startLat + (row - _row - 1) * resolution;//纬度

                            polygonStr += log.ToString() + " " + lat.ToString();
                            polygonStr += ",";
                        }
                        polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                        polygonStr += "),";
                    }
                    polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                    polygonStr += "),";
                }
                polygonStr = polygonStr.TrimEnd(',');//移除最后一个逗号
                polygonStr += ")";
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                //一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。
                //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
                //geoPolygon.AddGeometry(geoPolygon2);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);

                //释放资源
                geoPolygon.Dispose();
                oFeature.Dispose();
            }

            olayer1.Dispose();
            ds1.Dispose();
        }

        /// <summary>
        /// 计算一个经纬网格面积
        /// </summary>
        /// <param name="rasterStartLat">网格起始纬度,角度值</param>
        /// <param name="rasterEndLat">网格结束纬度,角度值</param>
        /// <param name="rasterLog">网格跨越经度范围</param>
        /// <returns>经纬网格实际面积，单位平方千米</returns>
        public static double GetRasterArea(double rasterStartLat, double rasterEndLat, double rasterLog)
        {
            double earthRadius = 6371.393;//单位千米
            double cutCount = 360.0 / rasterLog;//一个圆环被切割的份数

            //将地球视为球体的经纬1°网格计算公式为2πr²(sin(α+1)-sin(α))/360，半径为r,在纬度为α，https://www.zybang.com/question/deba669df1201d9c8d5c95e003716524.html
            double rasterArea = 2 * Math.PI * earthRadius* earthRadius * (Math.Sin(Math.PI * rasterEndLat / 180) - Math.Sin(Math.PI * rasterStartLat / 180)) / cutCount;//积分计算面积

            return rasterArea;
        }

        /// <summary>  
        /// 判断点是否在多边形内.  
        /// ----------原理----------  
        /// 注意到如果从P作水平向左的射线的话，如果P在多边形内部，那么这条射线与多边形的交点必为奇数，  
        /// 如果P在多边形外部，则交点个数必为偶数(0也在内)。  
        /// https://blog.csdn.net/xxdddail/article/details/49093635
        /// </summary>  
        /// <param name="checkPoint">要判断的点</param>  
        /// <param name="polygonPoints">多边形的顶点，首尾不同点</param>  
        /// <returns></returns>  
        private static bool IsInPolygon(Node checkPoint, List<Node> polygonPoints)
        {
            bool inside = false;
            int pointCount = polygonPoints.Count;
            Node p1, p2;
            for (int i = 1, j = 0; i < pointCount; j = i, i++)//(后面的话失效)第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点...  
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                if (checkPoint.row < p2.row)
                {//p2在射线之上  
                    if (p1.row <= checkPoint.row)
                    {//p1正好在射线中或者射线下方  
                        if ((checkPoint.row - p1.row) * (p2.col - p1.col) > (checkPoint.col - p1.col) * (p2.row - p1.row))//斜率判断,在P1和P2之间且在P1P2右侧  
                        {
                            //射线与多边形交点为奇数时则在多边形之内，若为偶数个交点时则在多边形之外。  
                            //由于inside初始值为false，即交点数为零。所以当有第一个交点时，则必为奇数，则在内部，此时为inside=(!inside)  
                            //所以当有第二个交点时，则必为偶数，则在外部，此时为inside=(!inside)  
                            inside = (!inside);
                        }
                    }
                }
                else if (checkPoint.row < p1.row)
                {
                    //p2正好在射线中或者在射线下方，p1在射线上  
                    if ((checkPoint.row - p1.row) * (p2.col - p1.col) < (checkPoint.col - p1.col) * (p2.row - p1.row))//斜率判断,在P1和P2之间且在P1P2右侧  
                    {
                        inside = (!inside);
                    }
                }
            }
            return inside;
        }

        /// <summary>
        /// 判断点是否在多边形内,由yang编写
        /// </summary>
        /// <param name="checkNode">要判断的点</param>
        /// <param name="nodes">多边形的顶点，首尾同点</param>
        /// <returns></returns>
        private static bool IsInPolygonNew(Node checkNode, List<Node> nodes)
        {
            bool inside = false;
            Node n1, n2;
            for (int i = 0; i < nodes.Count-1; i++)
            {
                n1 = nodes[i];
                n2 = nodes[i+1];
                if (n1.col > checkNode.col)
                {//右侧
                    if (n1.row >= checkNode.row && n2.row < checkNode.row)
                    {
                        inside = !inside;
                    }
                    else if(n1.row<checkNode.row && n2.row>=checkNode.row)
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        /// <summary>
        /// 边界点认为不在多边形内
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static bool IsInPolygonNoBorder(double row, double col, List<Node> nodes)
        {
            bool inside = false;
            Node n1, n2;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                n1 = nodes[i];
                n2 = nodes[i + 1];
                bool isOnLine = IsOnLine(row, col, n1.row, n1.col, n2.row, n2.col);
                if (isOnLine) return false;//边界点不认为在内
                if (n1.col > col)
                {//右侧
                    if (n1.row >= row && n2.row < row)
                    {
                        inside = !inside;
                    }
                    else if (n1.row < row && n2.row >= row)
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        private static bool IsOnLine(double x, double y, int x1, int y1, int x2, int y2)
        {
            if ((Equal(x, x1) && Equal(y, y1)) || (Equal(x, x2) && Equal(y, y2))) return true;//端点

            if(x>=Min(x1,x2)&&x<=Max(x1,x2)&&y>=Min(y1,y2)&&y<=Max(y1,y2))
            {
                if ((Equal(x, x1) && Equal(x1, x2)) || (Equal(y, y1) && Equal(y1, y2))) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        private static bool Equal(double v1, double v2)
        {
            double differ = Math.Abs(v1 - v2);
            if (differ < 2E-10) return true;
            else return false;
        }

        private static bool IsInPolygonNew(float row,float col, List<Node> nodes)
        {
            bool inside = false;
            Node n1, n2;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                n1 = nodes[i];
                n2 = nodes[i + 1];
                if (n1.col > col)
                {//右侧
                    if (n1.row >= row && n2.row < row)
                    {
                        inside = !inside;
                    }
                    else if (n1.row < row && n2.row >= row)
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        private static Node GetNextNode(Node tailNode, ref List<Node> nodes, ref int minRow, ref int minCol, ref int maxRow, ref int maxCol)
        {
            if(tailNode.outDir=="r")
            {//向右搜索
                for(int i=(tailNode.id+1);i<nodes.Count;i++)
                {//向后搜索
                    if(tailNode.row== nodes[i].row)
                    {//行号相同
                        if(nodes[i].isUsed==false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "l") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if(node.type == 5) node.outDir = "b";//第五种结点
                            else if(node.type==6) node.outDir = "t";//第六种结点
                            nodes[i] = node;//替换
                            //判断最大最小行列数
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            else if (tailNode.outDir == "b")
            {//向下搜索
                for (int i = (tailNode.id + 1); i < nodes.Count; i++)
                {//向后搜索
                    if (tailNode.col == nodes[i].col)
                    {//列号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            //node.isUsed = true;
                            //if (node.dir1 == "t")
                            //{
                            //    node.outDir = node.dir2;
                            //}
                            //else
                            //{
                            //    node.outDir = node.dir1;
                            //}
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "t") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "r";//第五种结点
                            else if (node.type == 6) node.outDir = "l";//第六种结点
                            nodes[i] = node;//替换
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            else if (tailNode.outDir == "l")
            {//向左搜索
                for (int i = (tailNode.id - 1); i >= 0; i--)
                {//向前搜索
                    if (tailNode.row == nodes[i].row)
                    {//行号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            //node.isUsed = true;
                            //if (node.dir1 == "r")
                            //{
                            //    node.outDir = node.dir2;
                            //}
                            //else
                            //{
                            //    node.outDir = node.dir1;
                            //}
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "r") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "t";//第五种结点
                            else if (node.type == 6) node.outDir = "b";//第六种结点
                            nodes[i] = node;//替换
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            else if (tailNode.outDir == "t")
            {//向左搜索
                for (int i = (tailNode.id - 1); i >= 0; i--)
                {//向前搜索
                    if (tailNode.col == nodes[i].col)
                    {//列号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            //node.isUsed = true;
                            //if (node.dir1 == "b")
                            //{
                            //    node.outDir = node.dir2;
                            //}
                            //else
                            //{
                            //    node.outDir = node.dir1;
                            //}
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "b") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "l";//第五种结点
                            else if (node.type == 6) node.outDir = "r";//第六种结点
                            nodes[i] = node;//替换
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            return new Node();//理论上不会执行
        }

        private static Node GetNextNode(Node tailNode, ref Node[] nodes, ref int minRow, ref int minCol, ref int maxRow, ref int maxCol)
        {
            if (tailNode.outDir == "r")
            {//向右搜索
                for (int i = (tailNode.id + 1); i < nodes.Length; i++)
                {//向后搜索
                    if (tailNode.row == nodes[i].row)
                    {//行号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "l") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "b";//第五种结点
                            else if (node.type == 6) node.outDir = "t";//第六种结点
                            nodes[i] = node;//替换
                            //判断最大最小行列数
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            else if (tailNode.outDir == "b")
            {//向下搜索
                for (int i = (tailNode.id + 1); i < nodes.Length; i++)
                {//向后搜索
                    if (tailNode.col == nodes[i].col)
                    {//列号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            //node.isUsed = true;
                            //if (node.dir1 == "t")
                            //{
                            //    node.outDir = node.dir2;
                            //}
                            //else
                            //{
                            //    node.outDir = node.dir1;
                            //}
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "t") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "r";//第五种结点
                            else if (node.type == 6) node.outDir = "l";//第六种结点
                            nodes[i] = node;//替换
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            else if (tailNode.outDir == "l")
            {//向左搜索
                for (int i = (tailNode.id - 1); i >= 0; i--)
                {//向前搜索
                    if (tailNode.row == nodes[i].row)
                    {//行号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            //node.isUsed = true;
                            //if (node.dir1 == "r")
                            //{
                            //    node.outDir = node.dir2;
                            //}
                            //else
                            //{
                            //    node.outDir = node.dir1;
                            //}
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "r") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "t";//第五种结点
                            else if (node.type == 6) node.outDir = "b";//第六种结点
                            nodes[i] = node;//替换
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            else if (tailNode.outDir == "t")
            {//向左搜索
                for (int i = (tailNode.id - 1); i >= 0; i--)
                {//向前搜索
                    if (tailNode.col == nodes[i].col)
                    {//列号相同
                        if (nodes[i].isUsed == false)
                        {//该节点没有被使用
                            Node node = nodes[i];
                            //node.isUsed = true;
                            //if (node.dir1 == "b")
                            //{
                            //    node.outDir = node.dir2;
                            //}
                            //else
                            //{
                            //    node.outDir = node.dir1;
                            //}
                            if (node.type <= 4)
                            {//前四种结点
                                node.isUsed = true;
                                if (node.dir1 == "b") node.outDir = node.dir2;
                                else node.outDir = node.dir1;
                            }
                            else if (node.type == 5) node.outDir = "l";//第五种结点
                            else if (node.type == 6) node.outDir = "r";//第六种结点
                            nodes[i] = node;//替换
                            if (node.row < minRow) minRow = node.row;//最小行数
                            else if (node.row > maxRow) maxRow = node.row;//最大行数
                            if (node.col < minCol) minCol = node.col;//最小列数
                            else if (node.col > maxCol) maxCol = node.col;//最大列数
                            //List<Node> nextNodes = GetNextNodes(node, ref nodes, ref minRow, ref minCol, ref maxRow, ref maxCol);
                            //nextNodes.Insert(0, node);//将该结点添加到头上
                            //return nextNodes;
                        }
                        return nodes[i];
                    }
                }
            }
            return new Node();//理论上不会执行
        }

        //点结构
        struct Node
        {
            public int id;//唯一标识
            public int type;//结点类型
            public int row;//经度
            public int col;//维度
            public string dir1;//方向1
            public string dir2;//方向2
            public string outDir;//线进入方向
            public bool isUsed;//是否被使用
            public int power;//强度
            public int stormID;//暴雨id
        }

        //线结构
        struct Line
        {
            public int id;//唯一标识
            public List<Node> nodes;//所有点
            public int minRow;//最小行数
            public int minCol;//最小列数
            public int maxRow;//最大行数
            public int maxCol;//最大列数
            public int type;//线类型，0为外环，1为内环
            public int power;//强度
            public int stormID;//暴雨id
            public double length;//长度
        }

        struct Polygon
        {
            public int id;//唯一标识
            public int stormID;//暴雨id
            public List<Line> lines;//所有线
            public int minRow;//最小行数
            public int minCol;//最小列数
            public int maxRow;//最大行数
            public int maxCol;//最大列数
            public double area;//实际面积
            public double avgRainfall;//平均降雨量
            public double volume;//降雨体积
            public double maxRainfall;//最大降雨量
            public double minRainfall;//最小降雨量
            public int power;//强度
            public bool isMulti;//是否是多面
            public double length;//周长
            public double coreRow;//重心行号
            public double coreCol;//重心列号
            public Rectangle minRec;//最小面积外包矩形
            public Circle minOutCir;//最小面积外接圆
            public Circle maxInCir;//最大面积内接圆

            public double intensityMean;
            public double intensityMax;
            public double intensityMin;
            public double uSpeed;
            public double vSpeed;
        }

        struct MultiPolygon
        {
            public int id;//唯一标识
            public int stormID;//暴雨id,也是唯一的
            public List<Polygon> polygons;//所有线
            public int minRow;//最小行数
            public int minCol;//最小列数
            public int maxRow;//最大行数
            public int maxCol;//最大列数
            public int power;//强度
            public double area;//实际面积
            public double avgRainfall;//平均降雨量
            public double maxRainfall;//最大降雨量
            public double minRainfall;//最小降雨量
            public double volume;//累计降雨量
        }

        public static void CreatPolygonShp()
        {
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = @"E:\rain\shp.shp";
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "…";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PolygonLayer", sr, wkbGeometryType.wkbPolygon, null);
            //接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("FieldID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("名称", FieldType.OFTString);
            oFieldName.SetWidth(50);
            olayer1.CreateField(oFieldName, 1);

            //创建x坐标字段
            FieldDefn oFieldX = new FieldDefn("x", FieldType.OFTReal);
            oFieldX.SetWidth(10);
            oFieldX.SetPrecision(8);
            olayer1.CreateField(oFieldX, 1);
            //创建y坐标字段
            FieldDefn oFieldY = new FieldDefn("y", FieldType.OFTReal);
            oFieldY.SetWidth(10);
            oFieldY.SetPrecision(8);
            olayer1.CreateField(oFieldY, 1);
            //创建z坐标字段
            FieldDefn oFieldZ = new FieldDefn("z", FieldType.OFTReal);
            oFieldZ.SetWidth(10);
            oFieldZ.SetPrecision(8);
            olayer1.CreateField(oFieldZ, 1);
            //写入第一条数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();
            Feature oFeature = new Feature(oDefn);
            oFeature.SetField(0, 0);
            oFeature.SetField(1, "Polygon1");
            oFeature.SetField(2, 489592.624);
            oFeature.SetField(3, 3804367.891);
            oFeature.SetField(4, 386.3);
            Geometry geoPolygon = Geometry.CreateFromWkt("POLYGON ((0 0,10 15,20 0,0 0),(5 2,15 2,10 10,5 2))");
            //shapefile中 的面状目标是由多个子环构成，每个子环是由至少四个顶点构成的封闭的、无自相交现象的环。对于含有岛的多边形，构成它的环有内外环之分，每个环的顶点的排列顺序或者方向说明了这个环到底是内环还是外环。一个内环的顶点是按照逆时针顺序排列的；而对于外环，它的顶点排列顺序是顺时针方向。如果一个多边形只由 一个环构成，那么它的顶点排列顺序肯定是顺时针方向。
            //Geometry geoPolygon2 = Geometry.CreateFromWkt("POLYGON ((2 2,10 10,15 2,2 2))");
            //geoPolygon.AddGeometry(geoPolygon2);
            oFeature.SetGeometry(geoPolygon);
            olayer1.CreateFeature(oFeature);

            //写入第二条数据
            Feature oFeature1 = new Feature(oDefn);
            oFeature1.SetField(0, 1);
            oFeature1.SetField(1, "Point2");
            oFeature1.SetField(2, 489602.624);
            oFeature1.SetField(3, 3804367.891);
            oFeature1.SetField(4, 389.3);

            geoPolygon = Geometry.CreateFromWkt("POLYGON ((100 100,120 100,110 115,100 100))");
            oFeature1.SetGeometry(geoPolygon);
            olayer1.CreateFeature(oFeature1);
            oFeature1.Dispose();
            olayer1.Dispose();
            ds1.Dispose();
        }

        public static void CreatPointShp()
        {
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            string strVectorFile1 = @"E:\strom\shp";
            Ogr.RegisterAll();

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "…";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer("PointLayer", sr, wkbGeometryType.wkbPoint, null);
            //接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("FieldID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldName = new FieldDefn("FieldName", FieldType.OFTString);
            oFieldName.SetWidth(50);
            olayer1.CreateField(oFieldName, 1);

            //创建x坐标字段
            FieldDefn oFieldX = new FieldDefn("x", FieldType.OFTReal);
            oFieldX.SetWidth(10);
            oFieldX.SetPrecision(8);
            olayer1.CreateField(oFieldX, 1);
            //创建y坐标字段
            FieldDefn oFieldY = new FieldDefn("y", FieldType.OFTReal);
            oFieldY.SetWidth(10);
            oFieldY.SetPrecision(8);
            olayer1.CreateField(oFieldY, 1);
            //创建z坐标字段
            FieldDefn oFieldZ = new FieldDefn("z", FieldType.OFTReal);
            oFieldZ.SetWidth(10);
            oFieldZ.SetPrecision(8);
            olayer1.CreateField(oFieldZ, 1);
            //写入第一条数据
            FeatureDefn oDefn = olayer1.GetLayerDefn();
            Feature oFeature = new Feature(oDefn);
            oFeature.SetField(0, 0);
            oFeature.SetField(1, "Point1");
            oFeature.SetField(2, 489592.624);
            oFeature.SetField(3, 3804367.891);
            oFeature.SetField(4, 386.3);
            Geometry geoPoint = new Geometry(OSGeo.OGR.wkbGeometryType.wkbPoint);
            geoPoint.AddPoint(489592.624, 3804367.891, 386.3);
            oFeature.SetGeometry(geoPoint);
            olayer1.CreateFeature(oFeature);

            //写入第二条数据
            Feature oFeature1 = new Feature(oDefn);
            oFeature1.SetField(0, 1);
            oFeature1.SetField(1, "Point2");
            oFeature1.SetField(2, 489602.624);
            oFeature1.SetField(3, 3804367.891);
            oFeature1.SetField(4, 389.3);

            geoPoint.AddPoint(489602.624, 3804367.891, 389.3);
            oFeature1.SetGeometry(geoPoint);
            olayer1.CreateFeature(oFeature1);
            oFeature1.Dispose();
            olayer1.Dispose();
            ds1.Dispose();
        }
        
        
    }
}
