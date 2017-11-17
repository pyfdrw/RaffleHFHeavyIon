using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace RaffleHFHeavyIon
{
    class RaffleHelper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        // 排数
        private int _rowNow = 0;

        public int RowNow
        {
            set
            {
                _rowNow = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RowNow"));
            }
            get { return _rowNow; }
        }

        // 列数
        private int _columnNow = 0;

        public int ColumnNow
        {
            set
            {
                _columnNow = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ColumnNow"));
            }
            get { return _columnNow; }
        }

        // 所有排的历史记录
        private ObservableCollection<double> _rowHistoryCollection = new ObservableCollection<double>();

        public ObservableCollection<double> RowHistoryCollection
        {
            set
            {
                _rowHistoryCollection = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RowHistoryCollection"));
            }
            get { return _rowHistoryCollection; }
        }

        // 所有列的历史记录
        private ObservableCollection<double> _columnHistoryCollection = new ObservableCollection<double>();

        public ObservableCollection<double> ColumnHistoryCollection
        {
            set
            {
                _columnHistoryCollection = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ColumnHistoryCollection"));
            }
            get { return _columnHistoryCollection; }
        }

        // 排数和列数的上限(可以取到的上限)
        private int _rowUpBound = 20;
        private int _columnUpBound = 20;

        // 随机数产生器
        private Random rdForRow;
        private Random rdForColumn;

        // 根据当前时间产生随机数，并且赋值给RowNow和ColumnNow,时间精确到毫秒
        private void generateRandom()
        {
            if(rdForRow == null || rdForColumn == null)
                throw new Exception("随机数未初始化!");
            try
            {
                int rdNumber = rdForRow.Next(_rowUpBound);
                rdNumber = rdForColumn.Next(_rowUpBound);
            }
            catch (Exception e)
            {
                throw new Exception("无法产生随机数!");
            }
            // 行数
            while (true)
            {
                int rdNumber = rdForColumn.Next(_rowUpBound);
                if (rdNumber != 0)
                {
                    RowNow = rdNumber;
                    break;
                }
            }
            //列数
            while (true)
            {
                int rdNumber = rdForColumn.Next(_columnUpBound);
                if (rdNumber != 0)
                {
                    ColumnNow = rdNumber;
                    break;
                }
            }
        }

        // 间隔一定时间不停产生随机数
        // timeGap毫秒数
        public void GenerateRandomSequentially(int timeGap = 50)
        {
            generateRandom();
            Thread.Sleep(timeGap);
        }

        // 记录历史
        public void AddHistory()
        {
            if (ColumnHistoryCollection != null && RowHistoryCollection != null)
            {
                RowHistoryCollection.Add(RowNow);
                ColumnHistoryCollection.Add(ColumnNow);
            }
            else
            {
                throw new Exception("初始化出现错误，记录历史的集合为空");
            }
        }

        // 初始化函数
        public RaffleHelper(int rowUpbound, int columnUpBound)
        {
            rdForRow = new Random(DateTime.Now.Second * 1000 + DateTime.Now.Millisecond);
            rdForColumn = new Random(DateTime.Now.Year * 1000 + DateTime.Now.Millisecond);

            _rowUpBound = rowUpbound;
            _columnUpBound = columnUpBound;
        }
    }
}
