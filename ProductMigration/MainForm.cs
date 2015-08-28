using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBAccess;
using BLL.gigade;
using BLL.gigade.Common;
using ProductMigration.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using System.Reflection;
using System.Threading;
using System.Collections;

namespace ProductMigration
{
    public partial class MainForm : Form
    {
        private IDBAccess _dbAccess = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openExcelDialog.ShowDialog();

        }

        private void openExcelDialog_FileOk(object sender, CancelEventArgs e)
        {
            textBox1.Text = openExcelDialog.FileName;
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            toolStripStatusLabel4.Text = "0";
            toolStripStatusLabel6.Text = "0";
        }

        public List<T> getObjByTable<T>(DataTable dt)
        {
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T local = (T)Activator.CreateInstance(typeof(T));
                
                PropertyInfo[] properties = local.GetType().GetProperties();
                if (properties != null)
                {
                    foreach (PropertyInfo info in properties)
                    {
                        MethodInfo setMethod = info.GetSetMethod();
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (info.Name.ToLower().Equals(column.ColumnName.ToLower()))
                            {
                                setMethod.Invoke(local, new object[] { row[column.ColumnName] == DBNull.Value ? "" : row[column.ColumnName] });
                            }
                        }
                    }
                }
                list.Add(local);
            }
            return list;
        }

        Thread thread;
        string mysqlConnectionStr = System.Configuration.ConfigurationSettings.AppSettings["MySqlConnectionString"];
        private void btn_sure_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            toolStripStatusLabel4.Text = "0";
            toolStripStatusLabel6.Text = "0";
            toolStripProgressBar1.Value = 0;
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("未指定文件"); return;
            }
            if (!System.IO.File.Exists(textBox1.Text))
            {
                MessageBox.Show("文件不存在"); return;
            }
            ThreadStart ts = new ThreadStart(Do);//給線程附上事件,執行Do這個事件
            thread = new Thread(ts);
            thread.Start();
        }
        //執行文件的解析,判斷Sheet的名稱來進行處理
        public void Do()
        {
            SetStatus(true);

            toolStripStatusLabel2.Text = "文件讀取中...";
            try
            {
                NPOI4ExcelHelper excelHelper = new NPOI4ExcelHelper(textBox1.Text);
                ArrayList name = excelHelper.SheetNameForExcel();
                if (name.IndexOf("單品") != -1)//名稱為單品
                {
                    toolStripStatusLabel2.Text = "單品讀取中...";
                    DataTable _dt = excelHelper.SheetData(name.IndexOf("單品"));

                    toolStripStatusLabel2.Text = "單品處理中...";
                    DataBaseOperation(_dt);//對單品的商品進行處理
                }
                if (name.IndexOf("組合") != -1)//名稱為組合
                {
                    toolStripStatusLabel4.Text = "0";
                    toolStripStatusLabel6.Text = "0";
                    toolStripProgressBar1.Value = 0;

                    toolStripStatusLabel2.Text = "組合讀取中...";
                    DataTable _dt = excelHelper.SheetData(name.IndexOf("組合"));
                    List<CombinationExcel> datas = getObjByTable<CombinationExcel>(_dt);
                    datas = datas.Skip(1).ToList();

                    toolStripProgressBar1.Maximum = datas.Count;
                    toolStripStatusLabel6.Text = datas.Count.ToString();
                    toolStripStatusLabel2.Text = "組合處理中...";

                    Combination combination = new Combination(mysqlConnectionStr, this);
                    combination.PrepareData(datas);

                    dataGridView2.DataSource = datas;
                    change(datas.Count - toolStripProgressBar1.Value);
                }
                toolStripStatusLabel2.Text = "完成";
            }
            catch (Exception ex)
            {
                log.Error("處理出錯", ex);
                toolStripStatusLabel2.Text = "出錯";
            }
            SetStatus(false);
            textBox1.Text = "";
        }

        public void SetStatus(bool status)
        {
            textBox1.ReadOnly = status;
            btn_sure.Enabled = !status;
            button1.Enabled = !status;
        }
        private delegate void ChangeDelegate(int value);
        public void change(int value)
        {
            if (toolStripProgressBar1.IsOnOverflow)
            {
                ChangeDelegate d = new ChangeDelegate(change);
                this.Invoke(d, value);
            }
            else
            {
                toolStripProgressBar1.Value += value;
                toolStripStatusLabel4.Text = toolStripProgressBar1.Value.ToString();
            }
        }

        public void DataBaseOperation(DataTable _dt)
        {
            List<MigrationDataSet> mds = null;
            _dt.Rows.RemoveAt(0);//移除第一行的標題文字

            mds = getObjByTable<MigrationDataSet>(_dt);

            if (mds != null)
            {
                toolStripProgressBar1.Maximum = mds.Count();
                toolStripStatusLabel6.Text = mds.Count().ToString();
                int errorCount = mds.Where(rec => !string.IsNullOrEmpty(rec.OutMessage)).ToList().Count();
                change(errorCount);
                
                List<MigrationDataSet> oldList = mds.Where(rec => rec.Is_exist == "old" && string.IsNullOrEmpty(rec.OutMessage) && !rec.Combination.Equals("199")).ToList();
                if (oldList != null && oldList.Count() > 0)
                {
                    DataOperation dOper = operationFactory.CreateOperation("old", oldList, this);
                    if (dOper.Exec())
                    {
                        mds.FindAll(rec => rec.Is_exist == "old" &&rec.Combination=="1" && string.IsNullOrEmpty(rec.OutMessage)).ForEach(rec => rec.OutMessage = "匯入成功");
                    }
                    else
                    {
                        mds.FindAll(rec => rec.Is_exist == "old" && rec.Combination == "1" && string.IsNullOrEmpty(rec.OutMessage)).ForEach(rec => rec.OutMessage = "匯入失敗");
                    }
                }
                List<MigrationDataSet> newList = mds.Where(rec => rec.Is_exist == "new" && string.IsNullOrEmpty(rec.OutMessage) && !rec.Combination.Equals("199")).ToList();
                if (newList != null && newList.Count() > 0)
                {
                    DataOperation dOper = operationFactory.CreateOperation("new", newList, this);
                    if (dOper.Exec())
                    {
                        mds.FindAll(rec => rec.Is_exist == "new" && rec.Combination == "1" && string.IsNullOrEmpty(rec.OutMessage)).ForEach(rec => rec.OutMessage = "匯入成功");
                    }
                    else
                    {
                        mds.FindAll(rec => rec.Is_exist == "new" && rec.Combination == "1" && string.IsNullOrEmpty(rec.OutMessage)).ForEach(rec => rec.OutMessage = "匯入失敗");
                    }
                }

                List<MigrationDataSet> priceList = mds.Where(rec => rec.Combination == "199" && string.IsNullOrEmpty(rec.OutMessage)).ToList();
                if (priceList != null && priceList.Count() > 0)
                {
                    DataOperation dOper = operationFactory.CreateOperation("199", priceList, this);
                    dOper.Exec();
                    //edit by hufeng0813w 2014/04/29
                    //if ()
                    //{
                    //    mds.FindAll(rec => rec.Combination == "199" && string.IsNullOrEmpty(rec.OutMessage)).ForEach(rec => rec.OutMessage = "匯入成功");
                    //}
                    //else
                    //{
                    //    mds.FindAll(rec => rec.Combination == "199" && string.IsNullOrEmpty(rec.OutMessage)).ForEach(rec => rec.OutMessage = "匯入失敗");
                    //}
                }

                dataGridView1.DataSource = mds;
                //dataGridView1.Refresh(); 
            }
        }

    }
}
