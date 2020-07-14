#pragma once

#include "Vector.h"

class c_ortho_basis
{

public:
	Vector data[3];
public:
	c_ortho_basis()
	{
	}

	c_ortho_basis(const Vector& u, const Vector& v, const Vector& w)
	{
		data[0] = u;
		data[1] = v;
		data[2] = w;
	}

	Vector& u()
	{
		return data[0];
	}

	Vector& v()
	{
		return data[1];
	}

	Vector& w()
	{
		return data[2];
	}

	const Vector& u() const
	{
		return data[0];
	}

	const Vector& v() const
	{
		return data[1];
	}

	const Vector& w() const
	{
		return data[2];
	}

	inline void set(const Vector& u, const Vector& v, const Vector& w)
	{
		data[0] = u;
		data[1] = v;
		data[2] = w;
	}

	void init_from_u(const Vector& u);
	void init_from_v(const Vector& v);
	void init_from_w(const Vector& w);

	void init_from_uv(const Vector& u, const Vector& v);
	void init_from_vu(const Vector& v, const Vector& u);

	void init_from_uw(const Vector& u, const Vector& w);
	void init_from_wu(const Vector& w, const Vector& u);

	void init_from_vw(const Vector& v, const Vector& w);
	void init_from_wv(const Vector& w, const Vector& v);

};

inline bool operator==(const c_ortho_basis& o1, const c_ortho_basis& o2)
{
	return (o1.data[0] == o2.data[0] && o1.data[1] == o2.data[1] && o1.data[2] == o2.data[2]);
}

inline bool operator!=(const c_ortho_basis& o1, const c_ortho_basis& o2)
{
	return (o1.data[0] != o2.data[0] || o1.data[1] != o2.data[1] || o1.data[2] != o2.data[2]);
}

inline std::istream& operator>>(std::istream& is, c_ortho_basis& o)
{
	Vector u, v, w;
	is >> u >> v >> w;
	o.init_from_uv(u, w);
	return is;
}

inline std::ostream& operator<<(std::ostream& os, const c_ortho_basis& o)
{
	return (os << o.data[0] << o.data[1] << o.data[2]);
}