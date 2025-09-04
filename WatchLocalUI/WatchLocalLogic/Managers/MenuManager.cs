using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch_Local.Managers
{
    public class MenuManager
    {
        public static int ShowMenu(string menu, int minValue, int maxValue)
        {
            Console.WriteLine(menu);
            string input = Console.ReadLine()!;
            int returnValue;
            while (input == null || !int.TryParse(input, out returnValue) || int.Parse(input) < minValue || int.Parse(input) > maxValue)
            {
                Console.WriteLine("Please select a valid option: \n");
                Console.WriteLine(menu);
                input = Console.ReadLine()!;
            }
            return returnValue;

        }
        public static int InputNumberVerificator(string input)
        {
            while (!int.TryParse(input, out _))
            {
                Console.WriteLine("Input a number:");
                input = Console.ReadLine()!;

            }
            int returnResult = int.Parse(input);
            return returnResult;

        }
        public static DateTimeOffset DatePicker()
        {
            Console.WriteLine("Year: ");
            int year = InputNumberVerificator(Console.ReadLine()!);
            Console.WriteLine("Month: ");
            int month = InputNumberVerificator(Console.ReadLine()!);
            while (month > 12 || month == 0)
            {
                Console.WriteLine("Enter a valid month:");
                month = InputNumberVerificator(Console.ReadLine()!);
            }
            Console.WriteLine("Day: ");
            int day = InputNumberVerificator(Console.ReadLine()!);
            var thirtyOneMonths = new List<int>() { 1, 3, 5, 7, 8, 10, 12 };
            var thirtyMonths = new List<int>() { 4, 6, 9, 11 };
            bool isValidDate = true;

            if (day < 1 && day > 31)
            {
                isValidDate = false;
            }
            else if (month == 2 && day > 28)
            {
                isValidDate = false;
            }
            else if (thirtyOneMonths.Contains(month) && day > 31)
            {
                isValidDate = false;
            }
            else if (thirtyMonths.Contains(month) && day > 30)
            {
                isValidDate = false;
            }
            while (!isValidDate)
            {
                // won't validate correctly
                Console.WriteLine("Enter a valid day for month selected");
                _ = int.TryParse(Console.ReadLine(), out day);
                isValidDate = true;
                if (day < 1 && day > 31)
                {
                    isValidDate = false;
                }
                else if (month == 2 && day > 28)
                {
                    isValidDate = false;
                }
                else if (thirtyOneMonths.Contains(month) && day > 31)
                {
                    isValidDate = false;
                }
                else if (thirtyMonths.Contains(month) && day > 30)
                {
                    isValidDate = false;
                }

            }
            var date = new DateTimeOffset(year, month, day, 0, 0, 0, new TimeSpan(0, 0, 0, 0));
            return date;
        }


    }
}
