#pragma once

template<class T>
class DynamicArray
{
public:
	DynamicArray()
	{
		count = 0;
		data_size = 4;
		data = new T[data_size];
	}

	DynamicArray(int s)
	{
		count = 0;
		data_size = s;
		data = new T[data_size];
	}

	~DynamicArray()
	{
		data_size = 0;
		if (data != 0)
			delete[] data;
	}

	DynamicArray(const DynamicArray<T>& rhs)
	{
		if (data != 0)
			delete[] data;

		count = rhs.length();
		data_size = rhs.data_size;
		data = new T[data_size];

		for (int i = 0; i < rhs.length(); i++)
			data[i] = rhs[i];
	}

	DynamicArray<T>& operator=(const DynamicArray<T>& rhs)
	{
		if (&rhs == this)
			return *this;

		if (data != null)
			delete[] data;

		count = rhs.length();
		data_size = rhs.data_size;
		data = new T[data_size];

		for (int i = 0; i < rhs.length(); i++)
			data[i] = rhs[i];

		return *this;
	}

	bool Append(T item)
	{
		if (count == data_size)
		{
			data_size *= 2;
			T* temp = data;
			if (!(data = new T[data_size]))
				return false;
			for (int i = 0; i < count; i++)
				data[i] = temp[i];
			delete temp;
		}

		data[count++] = item;
		return true;
	}

	bool Truncate()
	{
		if (count != data_size)
		{
			T* temp = data;
			data_size = count;
			if (!(data = new T[data_size]))
				return false;
			for (int i = 0; i < count; i++)
				data[i] = temp[i];
			delete temp;
		}
	}

	void Clear()
	{
		count = 0;
	}

	int Length() const
	{
		return count;
	}

	bool Empty() const
	{
		return length() == 0;
	}

	const T& operator[](int i) const
	{
		return data[i];
	}

	T& operator[](int i)
	{
		return data[i];
	}

private:
	T* data;
	int data_size;
	int count;
};

