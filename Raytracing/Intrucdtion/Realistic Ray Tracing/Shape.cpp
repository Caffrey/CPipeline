#include "Shape.h"

Triangle::Triangle(const Vector& _p0, const Vector& _p1, const Vector& _p2, const Color& _color) :
	p0(_p0), p1(_p1), p2(_p2), color(_color) {}


bool Triangle::hit(const Ray& r, RFLOAT tmin, RFLOAT tmax, RFLOAT time, HitRecord& record) const
{
	RFLOAT tval;
	RFLOAT A = p0.x() - p1.x();
	RFLOAT B = p0.y() - p1.y();
	RFLOAT C = p0.z() - p1.z();

	RFLOAT D = p0.x() - p2.x();
	RFLOAT E = p0.y() - p2.y();
	RFLOAT F = p0.z() - p2.z();

	RFLOAT G = r.Direction().x();
	RFLOAT H = r.Direction().y();
	RFLOAT I = r.Direction().z();

	RFLOAT J = p0.x() - r.Origin().x();
	RFLOAT K = p0.y() - r.Origin().y();
	RFLOAT L = p0.z() - r.Origin().z();

	RFLOAT EIHF = E * I - H * F;
	RFLOAT GFDI = G * F - D * I;
	RFLOAT DHEG = D * H - E * G;

	RFLOAT denom = (A * EIHF + B * GFDI + C * DHEG);
	RFLOAT beta = (J * EIHF + K * GFDI + L * DHEG) / denom;

	if (beta < 0.0f || beta >= 1.0f) return false;

	RFLOAT AKJB = A * K - J * B;
	RFLOAT JCAL = J * C - A * L;
	RFLOAT BLKC = B * L - K * C;

	RFLOAT gamma = (I * AKJB + H * JCAL + G * BLKC) / denom;
	if (gamma <= 0.0f || beta + gamma >= 1.0f) return false;

	tval = -(F * AKJB + E * JCAL + D * BLKC) / denom;
	if (tval >= tmin && tval <= tmax)
	{
		record.t = tval;
		record.normal = normalize(cross((p1 - p0), (p2 - p0)));
		record.color = color;
		return true;
	}
	return false;

}

bool Triangle::shadowHit(const Ray& r, RFLOAT tmin, RFLOAT tmax, RFLOAT time, HitRecord& record) const
{
	RFLOAT tval;
	RFLOAT A = p0.x() - p1.x();
	RFLOAT B = p0.y() - p1.y();
	RFLOAT C = p0.z() - p1.z();

	RFLOAT D = p0.x() - p2.x();
	RFLOAT E = p0.y() - p2.y();
	RFLOAT F = p0.z() - p2.z();

	RFLOAT G = r.Direction().x();
	RFLOAT H = r.Direction().y();
	RFLOAT I = r.Direction().z();

	RFLOAT J = p0.x() - r.Origin().x();
	RFLOAT K = p0.y() - r.Origin().y();
	RFLOAT L = p0.z() - r.Origin().z();

	RFLOAT EIHF = E * I - H * F;
	RFLOAT GFDI = G * F - D * I;
	RFLOAT DHEG = D * H - E * G;

	RFLOAT denom = (A * EIHF + B * GFDI + C * DHEG);
	RFLOAT beta = (J * EIHF + K * GFDI + L * DHEG) / denom;

	if (beta < 0.0f || beta >= 1.0f) return false;

	RFLOAT AKJB = A * K - J * B;
	RFLOAT JCAL = J * C - A * L;
	RFLOAT BLKC = B * L - K * C;

	RFLOAT gamma = (I * AKJB + H * JCAL + G * BLKC) / denom;
	if (gamma <= 0.0f || beta + gamma >= 1.0f) return false;

	tval = -(F * AKJB + E * JCAL + D * BLKC) / denom;

	return (tval >= tmin && tval <= tmax);
}

//-----------------Sphere


Sphere::Sphere(const Vector& _center, RFLOAT _radius, const Color& _color):
	center(_center),radius(_radius),color(_color){}


bool Sphere::hit(const Ray& r, RFLOAT tmin, RFLOAT tmax, RFLOAT time, HitRecord& record) const
{
	Vector temp = r.Origin() - center;
	double a = dot(r.Direction(), r.Direction());
	double b = 2 * dot(r.Direction(), temp);
	double c = dot(temp, temp) - radius * radius;

	double discriminant = b * b - 4 * a * c;
	if (discriminant <= 0)
	{
		return false;
	}

	discriminant = sqrt(discriminant);
	RFLOAT t = (-b - discriminant) / (2 * a);

	if (t < tmin)
		t = (-b + discriminant) / (2 * a);
	if (t < tmin || t > tmax)
		return false;

	record.t = t;
	record.normal = normalize(r.Origin() + t * r.Direction() - this->center);
	record.color = this->color;

	return true;
}

bool Sphere::shadowHit(const Ray& r, RFLOAT tmin, RFLOAT tmax, RFLOAT time, HitRecord& record) const
{
	Vector temp = r.Origin() - center;

	double a = dot(r.Direction(), r.Direction());
	double b = 2 * dot(r.Direction(), temp);
	double c = dot(temp, temp) - radius * radius;

	double discriminant = b * b - 4 * a * c;
	if (discriminant <= 0)
	{
		return false;
	}

	discriminant = sqrt(discriminant);
	RFLOAT t = (-b - discriminant) / (2 * a);

	if (t < tmin)
		t = (-b + discriminant) / (2 * a);
	if (t < tmin || t > tmax)
		return false;
}