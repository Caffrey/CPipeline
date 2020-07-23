#include "ortho_basis.h"

#define ONB_EPSILON 0.01f

#define U data[0]
#define V data[1]
#define W data[2]

void ONB::init_from_u(const Vector3& u)
{
	Vector3 n(1.0f, 0.0f, 0.0f);
	Vector3 m(0.0f, 1.0f, 0.0f);

	U = normalize(u);
	V = cross(U, n);
	if (V.length() < ONB_EPSILON)
		V = cross(U, m);
	W = cross(U, V);
}

void ONB::init_from_v(const Vector3& v)
{
	Vector3 n(1.0f, 0.0f, 0.0f);
	Vector3 m(0.0f, 1.0f, 0.0f);

	V = normalize(v);
	U = cross(V, n);
	if (U.length() < ONB_EPSILON)
		U = cross(V, m);
	W = cross(U, V);
}

void ONB::init_from_w(const Vector3& w)
{
	Vector3 n(1.0f, 0.0f, 0.0f);
	Vector3 m(0.0f, 1.0f, 0.0f);

	W = normalize(w);
	U = cross(W, n);
	if (U.length() < ONB_EPSILON)
		U = cross(W, m);
	V = cross(W, U);
}

void ONB::init_from_uv(const Vector3& u, const Vector3& v)
{
	U = normalize(u);
	W = normalize(cross(u, v));
	V = cross(W, U);
}

void ONB::init_from_vu(const Vector3& v, const Vector3& u)
{
	V = normalize(v);
	W = normalize(cross(u, v));
	U = cross(V, W);
}

void ONB::init_from_uw(const Vector3& u, const Vector3& w)
{
	U = normalize(u);
	V = normalize(cross(w, u));
	W = cross(U, W);
}

void ONB::init_from_wu(const Vector3& w, const Vector3& u)
{
	W = normalize(w);
	V = normalize(cross(w, u));
	U = cross(V, W);
}

void ONB::init_from_vw(const Vector3& v, const Vector3& w)
{
	V = normalize(v);
	U = normalize(cross(v, w));
	W = cross(U, V);
}

void ONB::init_from_wv(const Vector3& w, const Vector3& v)
{
	W = normalize(w);
	U = normalize(cross(v, w));
	V = cross(W, U);
}