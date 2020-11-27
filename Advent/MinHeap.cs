using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
	public class MinHeap<T> where T : IComparable<T>
	{

		private T[] m_elements;
		private int m_size;

		public MinHeap(int size)
		{
			m_elements = new T[size];
		}

		private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
		private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
		private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

		private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < m_size;
		private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < m_size;
		private bool IsRoot(int elementIndex) => elementIndex == 0;

		private T GetLeftChild(int elementIndex) => m_elements[GetLeftChildIndex(elementIndex)];
		private T GetRightChild(int elementIndex) => m_elements[GetRightChildIndex(elementIndex)];
		private T GetParent(int elementIndex) => m_elements[GetParentIndex(elementIndex)];

		private void Swap(int firstIndex, int secondIndex)
		{
			var temp = m_elements[firstIndex];
			m_elements[firstIndex] = m_elements[secondIndex];
			m_elements[secondIndex] = temp;
		}

		public bool IsEmpty()
		{
			return m_size == 0;
		}

		public int Count()
		{
			return m_size;
		}

		public T Peek()
		{
			if (IsEmpty())
				throw new IndexOutOfRangeException();

			return m_elements[0];
		}

		public T Pop()
		{
			if (IsEmpty())
				throw new IndexOutOfRangeException();

			var result = m_elements[0];
			m_elements[0] = m_elements[m_size - 1];
			m_size--;

			ReHeapDown();

			return result;
		}

		public void Add(T element)
		{
			if (m_size == m_elements.Length)
				throw new IndexOutOfRangeException();

			m_elements[m_size] = element;
			m_size++;

			ReHeapUp();
		}

		private void ReHeapDown()
		{
			int index = 0;
			while(HasLeftChild(index))
			{
				var smallerIndex = GetLeftChildIndex(index);
				if (HasRightChild(index) && GetRightChild(index).CompareTo(GetLeftChild(index)) < 0)
				{
					smallerIndex = GetRightChildIndex(index);
				}

				if (m_elements[smallerIndex].CompareTo(m_elements[index]) >= 0)
					break;

				Swap(smallerIndex, index);
				index = smallerIndex;
			}
		}

		private void ReHeapUp()
		{
			var index = m_size - 1;
			while(!IsRoot(index) && m_elements[index].CompareTo(GetParent(index)) < 0)
			{
				var parentIndex = GetParentIndex(index);
				Swap(parentIndex, index);
				index = parentIndex;
			}
		}
	}
}
