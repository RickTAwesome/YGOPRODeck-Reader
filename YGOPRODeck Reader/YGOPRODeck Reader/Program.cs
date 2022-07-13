using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YGOPRODeck_Reader
{
    public struct Card
    {
        public string name;
        public int[] id;
    }

    class Program
    {
        public static string[] files
        {
            get
            {
                DirectoryInfo directory = new DirectoryInfo(@"C:\Users\nghun\OneDrive\Desktop\Random Stuff\Everything\Programming\YGOPRODeck Reader\YGOPRODeck Reader\bin\Debug"); // Replace the empty string with the directory path to the debug folder that contains all the .ydk files you want to compare.

                FileInfo[] Files = directory.GetFiles("*.ydk");
                string[] fileNames = Files.Select(f => f.Name).ToArray();

                return fileNames;
            }
        }

        static void Main(string[] args)
        {
            int select;
            Database database = Database.Load("card_database.txt");
            LinkedList<int> list = Function.Count(files);

            string[] main = { 
                "What would you like to do?", 
                "0) Exit.", 
                "1) View decks.", 
                "2) Output the entire card database.", 
                "3) Output the amount of all cards used in every deck in total.", 
                "4) Output all cards in every deck with unrecognized IDs." 
            };

            #region Main Menu
            do
            {
                Console.WriteLine("The following files are up for comparison:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine(files[i]);
                }
                Console.WriteLine();
                for (int i = 0; i < main.Length; i++)
                {
                    Console.WriteLine(main[i]);
                }
                Console.WriteLine();
                Console.WriteLine("(Just write the number of the option you want to pick.)");
                Console.Write("Input: ");

                #region Check
                string input;
                do
                {
                    input = Console.ReadLine();
                    if (int.TryParse(input, out select))
                    {
                        if (select < main.Length - 1)
                        {
                            break;
                        }
                    }
                    InvalidInput();
                } while (int.TryParse(input, out select) || (select < main.Length - 1));
                Console.WriteLine();
                #endregion

                switch (select)
                {
                    case 0:
                        Console.WriteLine("Goodbye. Press any key to exit.");
                        break;
                    case 1:
                        Page.Option1();
                        break;
                    case 2:
                        Console.WriteLine("Card ID  - Card Name");
                        database.Write();
                        ProcessComplete();
                        break;
                    case 3:
                        Page.Option3();
                        ProcessComplete();
                        break;
                    case 4:
                        Console.WriteLine("Unrecognized IDs:");
                        if (list.Count != 0)
                        {
                            foreach (var id in list)
                            {
                                Console.WriteLine(id);
                            }
                        }
                        else
                        {
                            Console.WriteLine("All IDs recognized!");
                        }
                        ProcessComplete();
                        break;
                    default:
                        Console.WriteLine("Error. Not sure what happened. Press any key to continue.");
                        break;
                }
            } while (select != 0);
            #endregion

            Console.ReadKey();
        }

        public static void ProcessComplete()
        {
            Console.WriteLine();
            Console.WriteLine("Process complete. Press any key to return to menu.");
            Console.ReadKey();
            Console.Clear();
        }

        public static void InvalidInput()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 2);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine($"(Invalid input. Try again.)");
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("Input: ");
        }
    }

    public class Page
    {
        private struct ViewCard
        {
            public static int id;
            public static int count;

            public ViewCard(int id, int count)
            {
                ViewCard.id = id;
                ViewCard.count = count;
            }
        }

        public static void Option1() // Confusing as hell and definitely seems possible to optimize.
        {
            Console.Clear();
            Console.WriteLine("Choose which deck you want to view:");
            Console.WriteLine("0) Back to Main Menu");
            for (int i = 1; i < Program.files.Length; i++)
            {
                Console.WriteLine($"{i}) {Program.files[i]}");
            }
            Console.WriteLine();
            Console.Write("Input: ");

            #region Check
            string input;
            int select;
            do
            {
                input = Console.ReadLine();
                if (int.TryParse(input, out select))
                {
                    if (select < Program.files.Length - 1)
                    {
                        break;
                    }
                }
                Program.InvalidInput();
            } while (int.TryParse(input, out select) || (select < Program.files.Length - 1));
            Console.WriteLine();
            #endregion

            if (select == 0)
            {
                Console.Clear();
                return;
            }

            List<ViewCard> list = new List<ViewCard>();
            using (StreamReader reader = new StreamReader(Program.files[select]))
            {
                while (!reader.EndOfStream)
                {
                    string id = reader.ReadLine();
                    if (id.Length > 0)
                    {
                        if (id[0] != '#' && id[0] != '!')
                        {
                            if (list.Any(ViewCard => ViewCard.id == int.Parse(id)))
                            {
                                ViewCard.count++;
                            }
                            else
                            {
                                ViewCard viewCard = new ViewCard(int.Parse(id), 1);
                                list.Add(viewCard); // Can't be 'viewCard' since adding it to the list doesn't just store the information, but just item itself, which changes every few loops.
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Deck {select} ({Program.files[select]}) contains the following cards:");
            foreach (var item in list)
            {
                Console.WriteLine($"{ViewCard.count}x - {ViewCard.id}");
            }

            Program.ProcessComplete();
        }

        public static void Option3()
        {
            Console.WriteLine("Do you want the list sorted by descending amount of cards? (Y/N)");
            Console.Write("Input: ");

            string input;
            char select;
            do
            {
                input = Console.ReadLine();
                if (char.TryParse(input, out select))
                {
                    if (!(char.ToUpper(select) != 'Y') || !(char.ToUpper(select) != 'N')) // Double negatives, because I hate how '==' looks like.
                    {
                        break;
                    }
                }
                Program.InvalidInput();
            } while (!(char.TryParse(input, out select) && (!(char.ToUpper(select) != 'Y') || !(char.ToUpper(select) != 'N')))); // Probably could've made this slightly more efficient, but I'm too tired to figure it out.
            Console.WriteLine();

            Console.WriteLine($"Out of {Program.files.Length} decks there were...");
            Console.WriteLine();
            Console.WriteLine("Amount - Name - Ratio per deck");

            switch (char.ToLower(select))
            {
                case 'y':
                    Function.SortAndWrite();
                    break;
                case 'n':
                    Function.WriteCount(0);
                    break;
                default:
                    break;
            }
        }
    }

    public class Database
    {
        #region Linked List
        public class Node : IComparable<Node>
        {
            public Card carddata;
            public int count;
            public Node next;
            public Node prev;

            public Node()
            {

            }

            public Node(Card carddata, int count)
            {
                this.carddata = carddata;
                this.count = count;
                this.next = null;
                this.prev = null;
            }

            public int CompareTo(Node other)
            {
                return -count.CompareTo(other.count);
            }

            public override string ToString()
            {
                return $"{count}x - {carddata.name} ({(float)count / Program.files.Length})";
            }
        }

        public static Node head;
        public static Node tail;

        
        public static int Count
        {
            get
            {
                int anz = 0;

                for (Node current = head; current != null; current = current.next)
                {
                    anz++;
                }

                return anz;
            }
        }
        
        public Database()
        {
            head = null;
            tail = null;
        }
        #endregion

        #region Functions
        public static Database Load(string filename)
        {
            Database database = new Database();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line != "" && line[0] != '#')
                    {
                        string[] lines = line.Split('|');

                        Card card;
                        card.name = lines[0].Trim();
                        card.id = lines[1].Split(',').Select(int.Parse).ToArray();

                        database.AddLast(card, 0);
                    }
                }
            }

            return database;
        }

        public void AddLast(Card card, int count)
        {
            Node node = new Node(card, count);

            if (head == null)
            {
                head = tail = node;
                return;
            }

            node.prev = tail;
            tail.next = node;
            tail = node;
        }

        public void Write()
        {
            for (Node current = head; current != null; current = current.next)
            {
                Console.Write(current.carddata.id[0]);
                if (current.carddata.id[0].ToString().Length != 8)
                {
                    for (int i = 0; i < 8 - current.carddata.id[0].ToString().Length; i++)
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine($" - {current.carddata.name}");
            }
        }
        #endregion
    }

    public class Function
    {
        public static LinkedList<int> Count(string[] files)
        {
            LinkedList<int> list = new LinkedList<int>();

            for (int i1= 0; i1 < files.Length; i1++)
            {
                using (StreamReader reader = new StreamReader(files[i1]))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line != "" && line[0] != '#' && line[0] != '!')
                        {
                            for (Database.Node current = Database.head; current != null; current = current.next)
                            {
                                for (int i2 = 0; i2 < current.carddata.id.Length; i2++)
                                {
                                    if (current.carddata.id[i2] == int.Parse(line))
                                    {
                                        current.count++;
                                        goto label;
                                    }
                                    else if (current == Database.tail)
                                    {
                                        list.AddLast(int.Parse(line));
                                    }
                                }
                            }
                            label:;
                        }
                    }
                }
            }

            return list;
        }

        public static void WriteCount(int i)
        {
            for (Database.Node current = Database.head; current != null; current = current.next)
            {
                if (current.count > i)
                {
                    Console.WriteLine($"{current.count}x - {current.carddata.name}");
                }
            }
        }

        public static void SortAndWrite()
        {
            List<Database.Node> list = new List<Database.Node>();
            for (Database.Node current = Database.head; current != null; current = current.next)
            {
                list.Add(current);
            }
            list.Sort();

            foreach (var Node in list)
            {
                Console.WriteLine(Node);
            }
        }
    } // Essentially a pointless class, due to the fact you could move all the methods to the 'Database' class and save yourself the effort to make Nodes public and static, which basically also makes them pointless, but this looks cooler. And also makes me look stupider.
}
