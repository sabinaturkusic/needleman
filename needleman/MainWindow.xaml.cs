using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace needleman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string[] alignments = new string[2];
    


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var matchresult = nw();
            output.Text = alignments[0] + "\n"; 
            output.Text += alignments[1] + "\n";
            output.Text += string.Join("", matchresult); 
        }

        public string[] nw()
        {
            var result = Align(seqa.Text, seqb.Text, Convert.ToInt32(gap.Text), Convert.ToInt32(match.Text), Convert.ToInt32(mismatch.Text)); 

            var matchresult = PrintResult(seqb.Text, result[0]);
            return matchresult; 
        }

        public string[] PrintResult(string seqa, string nwseq)
        {
            string[] matchresult = new string[seqa.Length];

            for (int i = 0; i < seqa.Length; i++)
            {
                if (seqa[i] == nwseq[i])
                {
                    matchresult[i] = "*";
                }
                else
                {
                    matchresult[i] = "-";
                }
            }

            return matchresult; 
        }


        public int ScoreFunction(char a, char b, int matchScore, int mismatchScore)
        {

            return a == b ? matchScore : mismatchScore;
        }



        public string[] Align(string sequenceA, string sequenceB, int gapPenalty, int matchScore, int mismatchScore)
        {

            #region Initialize
            int[,] matrix = new int[sequenceA.Length + 1, sequenceB.Length + 1];
            char[,] tracebackMatrix = new char[sequenceA.Length + 1, sequenceB.Length + 1];
            matrix[0, 0] = 0;


            for (int i = 1; i < sequenceA.Length + 1; i++)
            {
                matrix[i, 0] = matrix[i - 1, 0] + gapPenalty;
                tracebackMatrix[i, 0] = 'L';
            }


            for (int i = 1; i < sequenceB.Length + 1; i++)
            {
                matrix[0, i] = matrix[0, i - 1] + gapPenalty;
                tracebackMatrix[0, i] = 'U';
            }
            #endregion


            #region Scoring
            for (int i = 1; i < sequenceA.Length + 1; i++)
            {
                for (int j = 1; j < sequenceB.Length + 1; j++)
                {

                    int diagonal = matrix[i - 1, j - 1] + ScoreFunction(sequenceA[i - 1], sequenceB[j - 1], matchScore, mismatchScore);
                    int links = matrix[i - 1, j] + gapPenalty;
                    int oben = matrix[i, j - 1] + gapPenalty;


                    matrix[i, j] = Math.Max(oben, Math.Max(links, diagonal));


                    if (matrix[i, j] == diagonal && i > 0 && j > 0)
                    {
                        tracebackMatrix[i, j] = 'D';
                    }
                    else if (matrix[i, j] == links)
                    {
                        tracebackMatrix[i, j] = 'L';
                    }
                    else if (matrix[i, j] == oben)
                    {
                        tracebackMatrix[i, j] = 'U';
                    }
                }
            }
            #endregion

            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);


            Console.Error.Write(matrix[sequenceA.Length, sequenceB.Length]);
            Console.Error.Write(Environment.NewLine);


            #region Traceback
            return TraceBack(tracebackMatrix, sequenceA, sequenceB);
            #endregion

        }

        public string[] TraceBack(char[,] tracebackMatrix, string sequenzA, string sequenzB)
        {

            int i = tracebackMatrix.GetLength(0) - 1;
            int j = tracebackMatrix.GetLength(1) - 1;

            StringBuilder alignedSeqA = new StringBuilder();
            StringBuilder alignedSeqB = new StringBuilder();


            while (tracebackMatrix[i, j] != 0)
            {
                switch (tracebackMatrix[i, j])
                {
                    case 'D':
                        alignedSeqA.Append(sequenzA[i - 1]);
                        alignedSeqB.Append(sequenzB[j - 1]);
                        i--;
                        j--;
                        break;
                    case 'U':
                        alignedSeqA.Append("-");
                        alignedSeqB.Append(sequenzB[j - 1]);
                        j--;
                        break;
                    case 'L':
                        alignedSeqA.Append(sequenzA[i - 1]);
                        alignedSeqB.Append("-");
                        i--;
                        break;

                }
            }

            

            alignments[0] = new string(alignedSeqA.ToString().Reverse().ToArray());
            alignments[1] = new string(alignedSeqB.ToString().Reverse().ToArray());


            return alignments;

        }
    }
}