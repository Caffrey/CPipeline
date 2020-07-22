#pragma once
#include "Ray.h"
#include "Vector.h"
#include "Color.h"

struct HitRecord
{
	float t;
	Vector normal;
	Color color;
};


class Shape
{
public :
	virtual bool hit(const Ray& r, float tmin, float tmax,  HitRecord& record) const = 0;
	virtual bool shadowHit(const Ray& r, float tmin, float tmax,  HitRecord& record) const = 0;
};

class Triangle : public Shape
{
public:
	Triangle(const Vector& _p0, const Vector& _p1, const Vector& _p2, const Color& _color);
	bool hit(const Ray& r, float tmin, float tmax, HitRecord& record) const ;
	bool shadowHit(const Ray& r, float tmin, float tmax, HitRecord& record) const ;

	Vector p0, p1, p2;
	Color color;
};


class Sphere :public Shape
{
public:

	Sphere(const Vector& _center, float _radius, const Color& _color);
	bool hit(const Ray& r, float tmin, float tmax, HitRecord& record) const;
	bool shadowHit(const Ray& r, float tmin, float tmax, HitRecord& record) const;
	//BBox boundingBox() const;

	Vector center;
	float radius;
	Color color;
};