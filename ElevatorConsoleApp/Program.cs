using System;
using System.Collections.Generic;
using System.Threading;

public class Elevator
{
    private Direction currentDirection = Direction.UP;
    private State currentState = State.IDLE;
    private int currentFloor = 0;

    Stack<Request> currentJobs = new Stack<Request>();
    Stack<Request> upPendingJobs = new Stack<Request>();
    Stack<Request> downPendingJobs = new Stack<Request>();
    public void startElevator()
    {
        Console.WriteLine("Welcome");
        Console.WriteLine("The Elevator has started functioning");
        while (true)
        {
            if (checkIfJob())
            {
                if (currentDirection == Direction.UP)
                {
                    Request request = currentJobs.Pop();
                    processUpRequest(request);
                    if (currentJobs.Count == 0)
                    {
                        addPendingDownJobsToCurrentJobs();
                    }
                }
                if (currentDirection == Direction.DOWN)
                {
                    Request request = currentJobs.Pop();
                    processDownRequest(request);
                    if (currentJobs.Count == 0)
                    {
                        addPendingUpJobsToCurrentJobs();
                    }
                }
            }
        }
    }
    public bool checkIfJob()
    {
        if (currentJobs.Count == 0)
        {
            return false;
        }
        return true;
    }
    public void processUpRequest(Request request)
    {
        int startFloor = currentFloor;
        // The elevator is not on the floor where the person has requested it i.e. source floor. So first bring it there.
        if (startFloor < request.GetExternalRequest().GetSourceFloor())
        {
            for (int i = startFloor; i <= request.GetExternalRequest().GetSourceFloor(); i++)
            {
                try
                {
                    Thread.Sleep(1000);
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                Console.WriteLine("We have reached floor -- " + i);
                currentFloor = i;
            }
        }
        // The elevator is now on the floor where the person has requested it i.e. source floor. User can enter and go to the destination floor.
        Console.WriteLine("Reached Source Floor--opening door");
        startFloor = currentFloor;
        for (int i = startFloor + 1; i <= request.GetInternalRequest().GetDestinationFloor(); i++)
        {
            try
            {
                Thread.Sleep(1000);
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            Console.WriteLine("We have reached floor -- " + i);
            currentFloor = i;
            if (checkIfNewJobCanBeProcessed(request))
            {
                break;
            }
        }
    }
    public void processDownRequest(Request request)
    {
        int startFloor = currentFloor;
        if (startFloor < request.GetExternalRequest().GetSourceFloor())
        {
            for (int i = startFloor; i <= request.GetExternalRequest().GetSourceFloor(); i++)
            {
                try
                {
                    Thread.Sleep(1000);
                }
                catch (ThreadInterruptedException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                Console.WriteLine("We have reached floor -- " + i);
                currentFloor = i;
            }
        }
        Console.WriteLine("Reached Source Floor--opening door");

        startFloor = currentFloor;

        for (int i = startFloor; i >= request.GetInternalRequest().GetDestinationFloor(); i--)
        {
            try
            {
                Thread.Sleep(1000);
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            Console.WriteLine("We have reached floor -- " + i);
            currentFloor = i;
            if (checkIfNewJobCanBeProcessed(request))
            {
                break;
            }
        }
    }
    public bool checkIfNewJobCanBeProcessed(Request currentRequest)
    {
        if (checkIfJob())
        {
            if (currentDirection == Direction.UP)
            {
                Request request = currentJobs.Pop();
                if (request.GetInternalRequest().GetDestinationFloor() < currentRequest.GetInternalRequest().GetDestinationFloor())
                {
                    currentJobs.Push(request);
                    currentJobs.Push(currentRequest);
                    return true;
                }
                currentJobs.Push(request);
            }
            if (currentDirection == Direction.DOWN)
            {
                Request request = currentJobs.Pop();
                if (request.GetInternalRequest().GetDestinationFloor() > currentRequest.GetInternalRequest().GetDestinationFloor())
                {
                    currentJobs.Push(request);
                    currentJobs.Push(currentRequest);
                    return true;
                }
                currentJobs.Push(request);
            }
        }
        return false;
    }
    public void addPendingDownJobsToCurrentJobs()
    {
        if (downPendingJobs.Count != 0)
        {
            Console.WriteLine("Pick a pending down job and execute it by putting in current job");
            currentJobs = downPendingJobs;
            currentDirection = Direction.DOWN;
        }
        else
        {
            currentState = State.IDLE;
            Console.WriteLine("The elevator is in Idle state");
        }
    }
    public void addPendingUpJobsToCurrentJobs()
    {
        if (upPendingJobs.Count != 0)
        {
            Console.WriteLine("Pick a pending up job and execute it by putting in current job");

            currentJobs = upPendingJobs;
            currentDirection = Direction.UP;
        }
        else
        {
            currentState = State.IDLE;
            Console.WriteLine("The elevator is in Idle state");
        }
    }
    public void addJob(Request request)
    {
        if (currentState == State.IDLE)
        {
            currentState = State.MOVING;
            currentDirection = request.GetExternalRequest().GetDirectionToGo();
            currentJobs.Push(request);
        }
        else if (currentState == State.MOVING)
        {
            if (request.GetExternalRequest().GetDirectionToGo() != currentDirection)
            {
                addtoPendingJobs(request);
            }
            else if (request.GetExternalRequest().GetDirectionToGo() == currentDirection)
            {
                if (currentDirection == Direction.UP && request.GetInternalRequest().GetDestinationFloor() < currentFloor)
                {
                    addtoPendingJobs(request);
                }
                else if (currentDirection == Direction.DOWN && request.GetInternalRequest().GetDestinationFloor() > currentFloor)
                {
                    addtoPendingJobs(request);
                }
                else
                {
                    currentJobs.Push(request);
                }
            }
        }
    }
    public void addtoPendingJobs(Request request)
    {
        if (request.GetExternalRequest().GetDirectionToGo() == Direction.UP)
        {
            Console.WriteLine("Add to pending up jobs");
            upPendingJobs.Push(request);
        }
        else
        {
            Console.WriteLine("Add to pending down jobs");
            downPendingJobs.Push(request);
        }
    }
}
public enum State
{
    MOVING,
    STOPPED,
    IDLE
}
public enum Direction
{
    UP,
    DOWN
}
public class Request : IComparable<Request>
{
    public InternalRequest _internalRequest;
    public ExternalRequest _externalRequest;
    public Request(InternalRequest internalRequest, ExternalRequest externalRequest)
    {
        this._internalRequest = internalRequest;
        this._externalRequest = externalRequest;
    }
    public InternalRequest GetInternalRequest()
    {
        return this._internalRequest;
    }
    public void SetInternalRequest(InternalRequest internalRequest)
    {
        this._internalRequest = internalRequest;
    }
    public ExternalRequest GetExternalRequest()
    {
        return this._externalRequest;
    }
    public void SetExternalRequest(ExternalRequest externalRequest)
    {

        this._externalRequest = externalRequest;
    }
    public int CompareTo(Request obj)
    {
        if (this._internalRequest.DestinationFloor > obj._internalRequest.DestinationFloor)
            return 1;
        else if (this._internalRequest.DestinationFloor < obj._internalRequest.DestinationFloor)
            return -1;
        else
            return 0;
    }

}
public class ProcessJobWorker
{
    private Elevator elevator;
    public ProcessJobWorker(Elevator elevator)
    {
        this.elevator = elevator;
    }

    public void run()
    {
        elevator.startElevator();
    }

}
public class AddJobWorker
{

    private Elevator elevator;
    private Request request;

    public AddJobWorker(Elevator elevator, Request request)
    {
        this.elevator = elevator;
        this.request = request;
    }

    public void run()
    {
        try
        {
            Thread.Sleep(1000);
        }
        catch (ThreadInterruptedException e)
        {
            Console.WriteLine(e.ToString());
            Console.Write(e.StackTrace);
        }
        elevator.addJob(request);
    }

}
public class ExternalRequest
{

    private Direction _directionToGo;
    private int _sourceFloor;

    public ExternalRequest(Direction directionToGo, int sourceFloor)
    {
        this._directionToGo = directionToGo;
        this._sourceFloor = sourceFloor;
    }
    public Direction GetDirectionToGo()
    {
        return this._directionToGo;
    }
    public void SetDirectionToGo(Direction directionToGo)
    {
        this._directionToGo = directionToGo;
    }

    public int GetSourceFloor()
    {
        return this._sourceFloor;
    }
    public void SetSourceFloor(int SourceFloor)
    {
        this._sourceFloor = SourceFloor;
    }

    public override string ToString()
    {
        return " The Elevator has been requested on floor - " + _sourceFloor + " and the person wants go in the - " + _directionToGo;
    }

}
public class InternalRequest
{
    public int DestinationFloor;
    public InternalRequest(int destinationFloor)
    {
        this.DestinationFloor = destinationFloor;
    }
    public int GetDestinationFloor()
    {
        return this.DestinationFloor;
    }
    public void SetDestinationFloor(int destinationFloor)

    {
        this.DestinationFloor = destinationFloor;
    }
    public override string ToString()
    {
        return "The destinationFloor is - " + DestinationFloor;
    }

}
public class Program
{
    public static void Main(string[] args)
    {

        Elevator elevator = new Elevator();
        ProcessJobWorker processJobWorker = new ProcessJobWorker(elevator);
        Thread t2 = new Thread(processJobWorker.run);
        t2.Start();

        try
        {
            Thread.Sleep(5000);
        }
        catch (ThreadInterruptedException e)
        {
            Console.WriteLine(e.ToString());
            Console.Write(e.StackTrace);
        }

        ExternalRequest er = new ExternalRequest(Direction.UP, 2);

        InternalRequest ir = new InternalRequest(5);

        Request request1 = new Request(ir, er);
        (new Thread(new AddJobWorker(elevator, request1).run)).Start();

        try
        {
            Thread.Sleep(5000);
        }
        catch (ThreadInterruptedException e)
        {
            Console.WriteLine(e.ToString());
            Console.Write(e.StackTrace);
        }


    }

}






