
//namespace ElevatorConsoleApp
//{
//    public class Program
//    {
//        private const string QUIT = "q";

//        public static void Main(string[] args)
//        {

//Start:
//            Console.WriteLine("Welcome");
//            Console.WriteLine("How many floors does this elevator will be needed?");

//            int floor; string floorInput; Elevator elevator;

//            floorInput = Console.ReadLine();

//            if (Int32.TryParse(floorInput, out floor))
//                elevator = new Elevator(floor);
//            else
//            {
//                Console.WriteLine("That' doesn't make sense...");
//                Console.Beep();
//                Thread.Sleep(5000);
//                Console.Clear();
//                goto Start;
//            }
//            string input = string.Empty;

//            while (input != QUIT)
//            {
//                Console.WriteLine("Please press which floor you would like to go to");

//                input = Console.ReadLine();
//                if (Int32.TryParse(input, out floor))
//                    elevator.FloorPress(floor);
//                else if (input == QUIT)
//                    Console.WriteLine("GoodBye!");
//                else
//                    Console.WriteLine("You have pressed an incorrect floor, Please try again");
//            }
//        }
//    }


//    public class Elevator
//    {
//        //The building building may has x numbers of floors

//        private bool[] floorReady;
//        public int CurrentFloor = 0;
//        private int topfloor;
//        public State Status = State.STOPPED;

//        public ElevatorRequest(int NumberOfFloors = 5)
//        {
//            floorReady = new bool[NumberOfFloors + 1];
//            topfloor = NumberOfFloors;

//        }

//        private void Stop(int floor)
//        {
//            Status = State.STOPPED;
//            CurrentFloor = floor;
//            floorReady[floor] = false;
//            Console.WriteLine("Stopped at floor {0}", floor);
//        }

//        //Button to close the doors
//        private void CloseDoors(int floor)
//        {
//            CurrentFloor = floor;
//            floorReady[floor] = false;

//        }

//        private void MovingDown(int floor)
//        {
//            for (int i = CurrentFloor; i >= 1; i--)
//            {
//                if (floorReady[i])
//                    Stop(floor);
//                else
//                    continue;
//            }

//            Status = State.STOPPED;
//            Console.WriteLine("Please Wait until the door is opened.");
//            Console.WriteLine("The door is Opened.");
//        }

//        private void MovingUp(int floor)
//        {
//            for (int i = CurrentFloor; i <= topfloor; i++)
//            {
//                if (floorReady[i])
//                    Stop(floor);
//                else
//                    continue;
//            }

//            Status = State.STOPPED;
//            Console.WriteLine("Please Wait until the door is opened.");
//            Console.WriteLine("The door is Opened.");
//        }

//        void StayPut()
//        {
//            Console.WriteLine("That's our current floor");
//        }

//        public void FloorPress(int floor)
//        {
//            if (floor > topfloor)
//            {
//                Console.WriteLine("We only have {0} floors", topfloor);
//                return;
//            }

//            floorReady[floor] = true;

//            switch (Status)
//            {

//                case State.DOWN:
//                    MovingDown(floor);
//                    break;

//                case State.STOPPED:
//                    if (CurrentFloor < floor)
//                        MovingUp(floor);
//                    else if (CurrentFloor == floor)
//                        StayPut();
//                    else
//                        MovingDown(floor);
//                    break;

//                case State.UP:
//                    MovingUp(floor);
//                    break;

//                default:
//                    break;
//            }


//        }

//        public enum State
//        {
//            UP,
//            STOPPED,
//            DOWN
//        }
//    }
//}
