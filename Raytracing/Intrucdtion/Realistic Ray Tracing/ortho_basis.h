#pragma once

#include "Vector.h"

class ONB
{

public:
	Vector3 data[3];
public:
	ONB()
	{
	}

	ONB(const Vector3& u, const Vector3& v, const Vector3& w)
	{
		data[0] = u;
		data[1] = v;
		data[2] = w;
	}

	Vector3& u()
	{
		return data[0];
	}

	Vector3& v()
	{
		return data[1];
	}

	Vector3& w()
	{
		return data[2];
	}

	const Vector3& u() const
	{
		return data[0];
	}

	const Vector3& v() const
	{
		return data[1];
	}

	const Vector3& w() const
	{
		return data[2];
	}

	inline void set(const Vector3& u, const Vector3& v, const Vector3& w)
	{
		data[0] = u;
		data[1] = v;
		data[2] = w;
	}

	void init_from_u(const Vector3& u);
	void init_from_v(const Vector3& v);
	void init_from_w(const Vector3& w);

	void init_from_uv(const Vector3& u, const Vector3& v);
	void init_from_vu(const Vector3& v, const Vector3& u);

	void init_from_uw(const Vector3& u, const Vector3& w);
	void init_from_wu(const Vector3& w, const Vector3& u);

	void init_from_vw(const Vector3& v, const Vector3& w);
	void init_from_wv(const Vector3& w, const Vector3& v);

};

inline bool operator==(const ONB& o1, const ONB& o2)
{
	return (o1.data[0] == o2.data[0] && o1.data[1] == o2.data[1] && o1.data[2] == o2.data[2]);
}

inline bool operator!=(const ONB& o1, const ONB& o2)
{
	return (o1.data[0] != o2.data[0] || o1.data[1] != o2.data[1] || o1.data[2] != o2.data[2]);
}

inline std::istream& operator>>(std::istream& is, ONB& o)
{
	Vector3 u, v, w;
	is >> u >> v >> w;
	o.init_from_uv(u, w);
	return is;
}

inline std::ostream& operator<<(std::ostream& os, const ONB& o)
{
	return (os << o.data[0] << o.data[1] << o.data[2]);
}