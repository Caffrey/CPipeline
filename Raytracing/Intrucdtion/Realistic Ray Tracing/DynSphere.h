#pragma once

#include "Common.h"
#include "Shape.h"
#include "Vector.h"
#include "Ray.h"
#include "Color.h"

class DynSphere : public Shape
{
	public:
		DynSphere(const Vector3& _ocenter, RFLOAT _radius,
			const Color& _color, RFLOAT min_time, RFLOAT max_time);
		bool hit(const Ray& r, float tmin, float tmax, RFLOAT time, HitRecord& record) const ;
		bool shadowHit(const Ray& r, float tmin, float tmax, RFLOAT time) const;

		Vector3 getCenter(RFLOAT time) const;


		Vector3 ocenter;
		RFLOAT mintime, maxtime, radius;
		Color color;

};

