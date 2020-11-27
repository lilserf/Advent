using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent
{
    class Menu
    {
        /// <summary>
        /// Run the menu on the console
        /// </summary>
        /// <typeparam name="T">Type of items to select, expected they will have ToString()</typeparam>
        /// <param name="items">Items to select from</param>
        /// <param name="initialSelection">Starting selection</param>
        /// <returns>Selection the user chose, or -1 to exit</returns>
        public static int RunMenu<T>(IEnumerable<T> items, int initialSelection)
        {
            const int Columns = 4;
            int selection = initialSelection;
            int ItemCount = items.Count();
            int itemsPerColumn = GetItemsPerColumn(ItemCount, Columns);
            bool wasCursorVisible = Console.CursorVisible;

            while (true)
            {
                Console.CursorVisible = false;
                PrintMenu(Columns, items, selection);

                var k = Console.ReadKey();
                if (k.Key == ConsoleKey.DownArrow)
                {
                    if (selection == ItemCount)
                    {
                        selection = 0;
                    }
                    else if ((selection % itemsPerColumn) == itemsPerColumn-1 || selection == ItemCount-1)
                    {
                        selection = ItemCount;
                    }
                    else
                    {
                        ++selection;
                    }
                }
                else if (k.Key == ConsoleKey.UpArrow)
                {
                    if (selection == ItemCount)
                    {
                        selection = itemsPerColumn - 1;
                    }
                    else if ((selection % itemsPerColumn) == 0)
                    {
                        selection = ItemCount;
                    }
                    else
                    {
                        --selection;
                    }
                }
                else if (k.Key == ConsoleKey.LeftArrow)
                {
                    if (selection == ItemCount)
                    {

                    }
                    else if (selection >= itemsPerColumn)
                    {
                        selection -= itemsPerColumn;
                    }
                }
                else if (k.Key == ConsoleKey.RightArrow)
                {
                    if (selection == ItemCount)
                    {

                    }
                    else if (selection / itemsPerColumn < Columns-1 && selection + itemsPerColumn < ItemCount)
                    {
                        selection += itemsPerColumn;
                    }
                }
                else if (k.Key == ConsoleKey.Enter)
                {
                    Console.CursorVisible = wasCursorVisible;
                    return (selection == ItemCount) ? -1 : selection;
                }
            }
        }

        private static void PrintMenu<T>(int columns, IEnumerable<T> items, int current)
        {
            Console.Clear();
            int columnWidth = Console.BufferWidth / columns;
            const string SelectionStart = "* ";
            const string SelectionEnd = " *";
            const string NonSelectionStart = "  ";
            const string NonSelectionEnd = NonSelectionStart;
            int itemCount = items.Count();
            int itemsPerColumn = GetItemsPerColumn(itemCount, columns);
            var enumer = items.GetEnumerator();
            for (int i = 0; i < columns; i = ++i)
            {
                for (int j = 0; j < itemsPerColumn; ++j)
                {
                    if (!enumer.MoveNext())
                    {
                        break;
                    }
                    int thisItemIndex = i * itemsPerColumn + j;
                    Console.CursorLeft = i * columnWidth;
                    Console.CursorTop = j;
                    Console.Write($"{(thisItemIndex == current ? SelectionStart : NonSelectionStart)}{enumer.Current.ToString()}{(thisItemIndex == current ? SelectionEnd : NonSelectionEnd)}");
                }
            }
            Console.CursorTop = itemsPerColumn;
            Console.CursorLeft = 0;
            Console.WriteLine($"{(current == itemCount ? SelectionStart : NonSelectionStart)}Exit{(current == itemCount ? SelectionEnd : NonSelectionEnd)}");
        }

        private static int GetItemsPerColumn(int numItems, int numColumns)
        {
            int itemsPerColumn = numItems / numColumns;
            if (numItems % numColumns != 0)
            {
                itemsPerColumn++;
            }
            return itemsPerColumn;
        }
    }
}
