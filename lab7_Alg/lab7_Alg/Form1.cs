using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Threading;
using System.Drawing;

namespace AISD_lab7
{
    public partial class Form1 : Form
    {
        int[] iArray;
        int MaxArraySize;
        private Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();
            chart1.Legends.Clear();
        }

        public delegate void SortingMethodDelegate(int[] iArray);

        class SortingFunction
        {
            public int[] iArray { get; set; }
            public SortingMethodDelegate SortingMethod { get; set; }
            public string sSortingMethod { get; set; }
            public double Time { get; set; }

            public void Sorting()
            {
                Stopwatch watch = Stopwatch.StartNew();
                SortingMethod(iArray);
                watch.Stop();
                Time = watch.Elapsed.TotalMilliseconds;
            }

            public void OutputToFile()
            {
                using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(sSortingMethod, false))
                {
                    foreach (var item in iArray)
                    {
                        file.WriteLine(item.ToString());
                    }
                }
            }
        }
        
        public void Test(SortingMethodDelegate SortingMethod, Series serie, string sSortingMethod)
        {
            int[] iTempArray = new int[iArray.Length];
            iArray.CopyTo(iTempArray, 0);

            SortingFunction SortingFunction = new SortingFunction();
            SortingFunction.iArray = iTempArray;
            SortingFunction.SortingMethod = SortingMethod;
            SortingFunction.sSortingMethod = sSortingMethod;

            Thread thread = new Thread(SortingFunction.Sorting);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            if (!thread.Join((int)numericUpDown4.Value))
            {
                thread.Abort();

                serie.Points.AddXY(sSortingMethod, (double)numericUpDown4.Value);
                serie.Points[serie.Points.Count - 1].Label = "more than " + numericUpDown4.Value.ToString();
                serie.Points[serie.Points.Count - 1].Color = randomColor;
                chart1.Legends["Legend"].CustomItems.Add(randomColor, sSortingMethod);

                Thread OutputThread2 = new Thread(SortingFunction.OutputToFile);
                OutputThread2.Priority = ThreadPriority.Normal;
                OutputThread2.Start();
                return;
            }
            else if (isCorrectlySorted(iTempArray))
            {
                throw new InvalidOperationException("Array isn't sorted!");
            }
            Thread OutputThread = new Thread(SortingFunction.OutputToFile);
            OutputThread.Priority = ThreadPriority.Normal;
            OutputThread.Start();

            serie.Points.AddXY(sSortingMethod, SortingFunction.Time);
            serie.Points[serie.Points.Count - 1].Label = SortingFunction.Time.ToString();
            serie.Points[serie.Points.Count - 1].Color = randomColor;
            chart1.Legends["Legend"].CustomItems.Add(randomColor, sSortingMethod);
        }

        static public void Swap<T>(ref T l, ref T r)
        {
            T temp = l;
            l = r;
            r = temp;
        }

        static public void Random(int[] iArray, int min, int max)
        {
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            for (int i = 0; i < iArray.Length; i++)
            {
                iArray[i] = random.Next(min, max);
            }
        }

        bool isCorrectlySorted(int[] iArray)
        {
            for (int i = 0; i < iArray.Length - 1; ++i)
            {
                if (iArray[i] > iArray[i + 1])
                {
                    return true;
                }
            }
            return false;
        }

        void BubbleSort(int[] iArray)
        {
            bool check;
            for (int i = 0; i < iArray.Length - 1; ++i)
            {
                check = true;
                for (int j = 0; j < iArray.Length - i - 1; ++j)
                {
                    if (iArray[j] > iArray[j + 1])
                    {
                        check = false;
                        Swap(ref iArray[j], ref iArray[j + 1]);
                    }
                }
                if (check)
                {
                    break;
                }
            }
        }

        void SelectionSort(int[] iArray)
        {
            int min;
            int indexmin;
            for (int i = 0; i < iArray.Length - 1; ++i)
            {
                min = iArray[i];
                indexmin = i;
                for (int j = i + 1; j < iArray.Length; ++j)
                {
                    if (min > iArray[j])
                    {
                        min = iArray[j];
                        indexmin = j;
                    }
                }
                Swap(ref iArray[i], ref iArray[indexmin]);
            }
        }

        void ShellSort(int[] iArray)
        {
            int temp;
            for (int step = iArray.Length / 2; step > 0; step /= 2)
            {
                for (int i = step; i < iArray.Length; ++i)
                {
                    temp = iArray[i];
                    int j;
                    for (j = i; j >= step && iArray[j - step] > temp; j -= step)
                    {
                        iArray[j] = iArray[j - step];
                    }
                    iArray[j] = temp;
                }
            }
        }

        void MergeSort(int[] iArray)
        {
            MergeSort(iArray, 0, iArray.Length - 1);
        }

        void MergeSort(int[] iArray, int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;
                MergeSort(iArray, left, middle);
                MergeSort(iArray, middle + 1, right);
                Merge(iArray, left, right, middle);
            }
        }
       
        void Merge(int[] iArray, int left, int right, int middle)
        {
            int lsize = middle - left + 1;
            int rsize = right - middle;
            int[] iArray_l = new int[lsize];
            int[] iArray_r = new int[rsize];

            int i;
            int j;

            for (i = left, j = 0; i <= middle; ++i, ++j)
            {
                iArray_l[j] = iArray[i];
            }
            for (i = middle + 1, j = 0; i <= right; ++i, ++j)
            {
                iArray_r[j] = iArray[i];
            }

            int k = left;
            i = 0;
            j = 0;

            while (i < lsize && j < rsize)
            {
                if (iArray_l[i] >= iArray_r[j])
                {
                    iArray[k++] = iArray_r[j++];
                }
                else
                {
                    iArray[k++] = iArray_l[i++];
                }
            }
            while (i < lsize)
            {
                iArray[k++] = iArray_l[i++];
            }
            while (j < rsize)
            {
                iArray[k++] = iArray_r[j++];
            }
        }
        
        void QuickSort(int[] iArray)
        {
            QuickSort(iArray, 0, iArray.Length - 1);
        }

        void QuickSort(int[] iArray, int left, int right)
        {
            int i = left, j = right;
            int middle = iArray[(i + j) / 2];
            while (i <= j)
            {
                while (iArray[i] < middle)
                {
                    i++;
                }
                while (iArray[j] > middle)
                {
                    j--;
                }
                if (i <= j)
                {
                    Swap(ref iArray[i], ref iArray[j]);
                    i++;
                    j--;
                }
            }
            if (left < j)
            {
                QuickSort(iArray, left, j);
            }
            if (i < right)
            {
                QuickSort(iArray, i, right);
            }
        }

        void CountingSort(int[] iArray)
        {
            int min = iArray.Min();
            int[] aux = new int[iArray.Max() - min + 1];
            for (int i = 0; i < iArray.Length; ++i)
            {
                ++aux[iArray[i] - min];
            }
            for (int i = 1; i < aux.Length; ++i)
            {
                aux[i] += aux[i - 1];
            }
            int[] res = (int[])iArray.Clone();

            for (int i = res.Length - 1; i > -1; --i)
            {
                iArray[--aux[res[i] - min]] = res[i];
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            MaxArraySize = (int)numericUpDown1.Value;
            iArray = new int[MaxArraySize];
            Random(iArray, (int)numericUpDown2.Value, (int)numericUpDown3.Value);
            chart1.Series.Clear();
            chart1.Legends.Clear();

            Series MainSerie = chart1.Series.Add("Legend:");
            chart1.Legends.Add("Legend");
            MainSerie.Legend = "Legend";

            if(checkBox1.Checked)
            {
                Test(BubbleSort, MainSerie, "Bubble Sort");
            }
            if (checkBox2.Checked)
            {
                Test(SelectionSort, MainSerie, "Selection Sort");
            }
            if (checkBox3.Checked)
            {
                Test(ShellSort, MainSerie, "Shell Sort");
            }
            if (checkBox4.Checked)
            {
                Test(MergeSort, MainSerie, "Merge Sort");
            }
            if (checkBox5.Checked)
            {
                Test(QuickSort, MainSerie, "Quick Sort");
            }
            if (checkBox6.Checked)
            {
                Test(CountingSort, MainSerie, "Counting Sort");
            }
        }
    }

}


