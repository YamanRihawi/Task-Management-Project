using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Task
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public DateTime Date { get; set; }

        public Task(string name, int priority, DateTime date)
        {
            Name = name;
            Priority = priority;
            Date = date;
        }
    }

    class CompletedTask
    {
        public Task TaskData { get; set; }
        public CompletedTask Next { get; set; }

    }

    class Operation
    {
        public string Type { get; set; }
        public Task TaskData { get; set; }
        public int TaskIndex { get; set; }
    }

    class TaskManger
    {
        static Task[] tasks = new Task[100]; 
        static int taskCount = 0;             
        static CompletedTask completedHead = null;
        static List<Task> deletedTasks = new List<Task>();
        static Queue<Task> urgentTasks = new Queue<Task>();
        static Stack<Operation> operationStack = new Stack<Operation>();

        static void Main(string[] args)
        {
            while (true) 
            {
                Console.WriteLine("\n=== Task Management System ===");
                Console.WriteLine("1. Add New Task");
                Console.WriteLine("2. Display All Tasks");
                Console.WriteLine("3. Delete Task");
                Console.WriteLine("4. Display Deleted Tasks");
                Console.WriteLine("5. Sort Tasks by Priority");
                Console.WriteLine("6. Sort Tasks by Date");
                Console.WriteLine("7. Complete Task");
                Console.WriteLine("8. Display Completed Tasks");
                Console.WriteLine("9. Add Urgent Task");
                Console.WriteLine("10. Display Urgent Tasks");
                Console.WriteLine("11. Undo Last Operation");
                Console.WriteLine("12. Exit");
                Console.Write("\nEnter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        DisplayTasks();
                        break;
                    case "3":
                        DeleteTask();
                        break;
                    case "4":
                        DisplayDeletedTasks();
                        break;
                    case "5":
                        SortTasksByPriority();
                        break;
                    case "6":
                        SortTasksByDate();
                        break;
                    case "7":
                        CompleteTask();
                        break;
                    case "8":
                        DisplayCompletedTasks();
                        break;
                    case "9":
                        AddUrgentTask();
                        break;
                    case "10":
                        DisplayUrgentTasks();
                        break;
                    case "11":
                        UndoLastOperation();
                        break;
                    case "12":
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice Please try again");
                        break;
                }
            }
        }

        static void AddTask()
        {
            Console.Write("Enter task name: ");
            string name = Console.ReadLine();

            Console.Write("Enter priority (1-High 2-Medium 3-Low): ");
            int priority = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter creation date (yyyy-mm-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine());

            Task newTask = new Task(name, priority, date);
            tasks[taskCount] = newTask;

            Operation op = new Operation
            {
                Type = "Add",
                TaskData = newTask
            };
            operationStack.Push(op);

            taskCount++;

            Console.WriteLine("\nTask added successfully");
        }
    
        static void DisplayTasks()
        {
            if(taskCount == 0)
            {
                Console.WriteLine("\nno tasks availble");
                return;
            }
            Console.WriteLine("\n=== Task List ===");
            for (int i = 0; i < taskCount; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {tasks[i].Name}, Priority: {tasks[i].Priority}, Date: {tasks[i].Date.ToShortDateString()}");
            }
        }

        static void DeleteTask()
        {
            DisplayTasks();
            Console.Write("\nEnter the task number to delete: ");
            int index = int.Parse(Console.ReadLine());

            if (index > 0 && index <= taskCount)
            {
                Task deletedTask = tasks[index - 1];
                int deletedTaskIndex = index - 1;

                for (int i = index - 1; i < taskCount - 1; i++)
                {
                    tasks[i] = tasks[i + 1];
                }

                tasks[taskCount-1] = null;
                taskCount--;

                Operation op = new Operation
                {
                    Type = "Delete",
                    TaskData = deletedTask,
                    TaskIndex = deletedTaskIndex
                };
                operationStack.Push(op);

                deletedTasks.Add(deletedTask);

                Console.WriteLine("\nTask deleted successfully");  
            }
            else
            {
                Console.WriteLine("\nInvalid task number");
            }
        }

        static void DisplayDeletedTasks()
        {
            if (deletedTasks.Count == 0)
            {
                Console.WriteLine("No tasks have been deleted");
                return;
            }

            Console.WriteLine("\n=== Deleted Task List ====");
            for (int i = 0; i < deletedTasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Name: {deletedTasks[i].Name}, Priority: {deletedTasks[i].Priority}, Date: {deletedTasks[i].Date.ToShortDateString()}");
            }
        }

        static void SortTasksByPriority()
        {
            for (int i = 0; i < taskCount - 1; i++)
            {
                for (int j = 0; j < taskCount - i - 1; j++)
                {
                    if (tasks[j].Priority > tasks[j + 1].Priority)
                    {
                        Task temp = tasks[j];
                        tasks[j] = tasks[j + 1];
                        tasks[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine("\nTasks sorted by priority");
            DisplayTasks();

        }

        static void SortTasksByDate()
        {
            QuickSort(tasks, 0, taskCount - 1);

            Console.WriteLine("\nTasks sorted by date");
            DisplayTasks();
        }

        static void QuickSort(Task[] task, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(task, low, high);
                QuickSort(task, low, pivotIndex - 1);
                QuickSort(task, pivotIndex + 1, high);
            }
        }

        static int Partition(Task[] task, int low, int high)
        {
            Task pivot = task[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (task[j].Date < pivot.Date)
                {
                    i++;
                    Task temp = task[i];
                    task[i] = task[j];
                    task[j] = temp;
                }
            }

            Task temp2 = task[i + 1];
            task[i + 1] = task[high];
            task[high] = temp2;

            return i + 1;
        }

        static void CompleteTask()
        {
            DisplayTasks();
            Console.Write("Enter the task number to mark as complete: ");
            int index = int.Parse(Console.ReadLine());

            if (index > 0 && index <= taskCount)
            {
                Task completedTask = tasks[index - 1];

                for (int i = index - 1; i < taskCount - 1; i++)
                {
                    tasks[i] = tasks[i + 1];
                }
                tasks[taskCount - 1] = null;
                taskCount--;

                CompletedTask newCompletedTask = new CompletedTask();
                newCompletedTask.TaskData = completedTask;
                newCompletedTask.Next = completedHead;
                completedHead = newCompletedTask;

                Console.WriteLine("\nTask completed successfully");
                DisplayCompletedTasks();
            }
            else
            {
                Console.WriteLine("\nInvalid task number");
            }
        }

        static void DisplayCompletedTasks()
        {
            Console.WriteLine("\n=== Completed Tasks ===");
            CompletedTask current = completedHead;
            int count = 1;
            while (current != null)
            {
                Console.WriteLine($"{count}. Name: {current.TaskData.Name}, Priority: {current.TaskData.Priority}, Date: {current.TaskData.Date.ToShortDateString()}");
                current = current.Next;
                count++;
            }
        }

        static void AddUrgentTask()
        {
            Console.Write("Enter urgent task name: ");
            string name = Console.ReadLine();

            Console.Write("Enter priority (1-High, 2-Medium, 3-Low): ");
            int priority = int.Parse(Console.ReadLine());

            Console.Write("Enter creation date (yyyy-mm-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine());

            Task urgentTask = new Task(name, priority, date);
            urgentTasks.Enqueue(urgentTask);

            Console.WriteLine("\nUrgent task added");
            DisplayUrgentTasks();
        }

        static void DisplayUrgentTasks()
        {
            Console.WriteLine("\n=== Urgent Tasks ===");
            int count = 1;
            foreach (Task task in urgentTasks)
            {
                Console.WriteLine($"{count}. Name: {task.Name}, Priority: {task.Priority}, Date: {task.Date.ToShortDateString()}");
                count++;
            }
        }

        static void UndoLastOperation()
        {
            Operation lastOp = operationStack.Pop();

            if (lastOp.Type == "Add")
            {
                taskCount--;
                tasks[taskCount] = null;

                Console.WriteLine("\nLast add operation undone");
                DisplayTasks();

            }
            else if (lastOp.Type == "Delete")
            {
                for (int i = taskCount; i > lastOp.TaskIndex; i--)
                {
                    tasks[i] = tasks[i - 1];
                }
                tasks[lastOp.TaskIndex] = lastOp.TaskData;
                taskCount++;

                deletedTasks.Remove(lastOp.TaskData);

                Console.WriteLine("\nLast delete operation undone");
                DisplayTasks();
            }
        }
    }
}