#pragma once
#include "Vector.h"
#include <iostream>
class Ray
{
public:
	Ray();
	Ray(const Vector3& a, const Vector3& b) {
		data[0] = a;
		data[1] = b;
	}
	Ray(const Ray& r) { *this = r; }
	Vector3 Origin() const { return data[0]; }
	Vector3 Direction() const { return data[1]; }
	Vector3 potinAtParamter(float t)const
	{
		return data[0] + t * data[1];
	}
	
	Vector3 data[2];

};


inline std::ostream& operator << (std::ostream& os, const Ray& r)
{
	os << "(" << r.Origin() << ") + t(" << r.Direction() << ")";
	return os;
}