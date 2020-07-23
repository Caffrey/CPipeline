#include "DynSphere.h"
#include <math.h>
DynSphere::DynSphere(const Vector3& _ocenter, RFLOAT _radius, const Color& _color, RFLOAT min_time, RFLOAT max_time)
	:ocenter(_ocenter),radius(_radius),color(_color),mintime(min_time),maxtime(max_time)
{

}

bool DynSphere::hit(const Ray& r, float tmin, float tmax, RFLOAT time, HitRecord& record) const
{
	Vector3 new_center = getCenter(time);
	Vector3 temp = r.Origin() - new_center;
	
	double a = dot(r.Direction(), r.Direction());
	double b = 2 * dot(r.Direction(), temp);
	double c = dot(temp, temp) - radius * radius;

	double discriminant = b * b - 4 * a * c;
	if (discriminant > 0)
	{
		discriminant = sqrt(discriminant);
		double t = (-b - discriminant) / (2 * a);
		
		if (t < tmin)
			t = (-b + discriminant) / (2 * a);
		if (t < tmin || t >tmax)
			return false;

		record.t = t;
		record.normal = normalize(r.Origin() + t * r.Direction() - new_center);
		record.color = color;
		return true;
	}
	return false;
}

bool DynSphere::shadowHit(const Ray& r, float tmin, float tmax, RFLOAT time) const
{
	Vector3 new_center = getCenter(time);
	Vector3 temp = r.Origin() - new_center;

	double a = dot(r.Direction(), r.Direction());
	double b = 2 * dot(r.Direction(), temp);
	double c = dot(temp, temp) - radius * radius;

	double discriminant = b * b - 4 * a * c;
	if (discriminant > 0)
	{
		discriminant = sqrt(discriminant);
		double t = (-b - discriminant) / (2 * a);

		if (t < tmin)
			t = (-b + discriminant) / (2 * a);
		if (t < tmin || t >tmax)
			return false;

		return true;
	}
	return false;
}

Vector3 DynSphere::getCenter(RFLOAT time) const
{
	RFLOAT realtime = time * maxtime + (1.0f - time) * mintime;
	return Vector3(
		ocenter.x() + realtime,
		ocenter.y() + realtime,
		ocenter.z() + realtime
	);
}
