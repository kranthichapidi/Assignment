using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeAndMotionFinalAssignment
{
    class TimeAndMotionViewModel : INotifyPropertyChanged
    {
        public const int LOWER_LIMIT = 27;
        public const int UPPER_LIMIT = 127;
        public const int MINS_PER_HDAY = 720;//60 MIN*12 HOURS
        public const int CALC_PER_DAY = 2;//60*24
        public Stack<int> ones = new Stack<int>(5); //for 1st 5 -- min clock size
        public Stack<int> fives = new Stack<int>(12); //for 2nd 5 -- min clock size
        public Stack<int> hours = new Stack<int>(12); //for hours clock size
        public Queue<int> queue;//no.of ball 
        int[] StartingState; // to keep the balls state
        int cycles; //number of cycles made by the ball clock
        bool checkFlag;
        public int Input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
                CalcTM();
                OnPropertyChanged();
            }
        }
        public int _input;
        public string Output
        {
            get
            {
                return _output;
            }
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }
        public string _output;
        public string ErrorMsg
        {
            get
            {
                return _errmsg;
            }
            set
            {
                _errmsg = value;
                OnPropertyChanged();
            }
        }
        public string _errmsg;
        public TimeAndMotionViewModel()
        {
            Output = "Please enter any number between 27 and 127";
        }
        public void Reset()
        {
            queue = new Queue<int>();
            cycles = 0;
            checkFlag = false;
            StartingState = new int[Input];
        }
        public void CalcTM()
        {
            if (Input >= LOWER_LIMIT && Input <= UPPER_LIMIT)
            {
                Reset();
                for (int i = 1; i <= Input; i++)
                    queue.Enqueue(i);
                queue.CopyTo(StartingState, 0);
                do
                {
                    cycles = ClockMainFun(cycles);
                    checkFlag = QueueOrderCheck();
                } while (checkFlag);
                Output = Input + " balls cycles after " + (cycles / CALC_PER_DAY) + " days.";
            }
            else
                Output = "Invalid Input.";
        }
        public int ClockMainFun(int counter)
        {
            int mintBall;
            for (int i = 0; i < MINS_PER_HDAY; i++)
            {
                mintBall = AddBallToFiveSecQ();
                if (ones.Count < 4)
                    ones.Push(mintBall);
                else if (ones.Count == 4 && fives.Count < 11)
                {
                    fives.Push(mintBall);
                    DumpBackToTray(ones, 4);
                }
                else if (ones.Count == 4 && fives.Count == 11 && hours.Count < 11)
                {
                    hours.Push(mintBall);
                    DumpBackToTray(ones, 4);
                    DumpBackToTray(fives, 11);
                }
                else
                {
                    DumpBackToTray(ones, 4);
                    DumpBackToTray(fives, 11);
                    DumpBackToTray(hours, 11);
                    queue.Enqueue(mintBall);
                    counter = counter + 1;
                }
            }
            return counter;
        }
        public int AddBallToFiveSecQ()
        {
            int Ball = (int)queue.Dequeue();
            return Ball;
        }
        public void DumpBackToTray(Stack<int> stk, int counter)
        {
            for (int i = 0; i < counter; i++)
                queue.Enqueue(stk.Pop());
        }
        public bool QueueOrderCheck()
        {
            int counter = 0;
            int[] compare = new int[Input];
            queue.CopyTo(compare, 0);
            for (int i = 0; i < Input; i++)
            {
                if (compare[i] == StartingState[i])
                    counter++;
                if (counter == Input)
                    return false;
            }
            return true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
