#include "Camera.h"
#include "Ray.h"
Camera::Camera()
{

}

Camera::Camera(const Camera& orig)
{
	center = orig.center;
	corner = orig.corner;
	across = orig.across;

	up = orig.up;
	uvw = orig.uvw;
	lens_radius = orig.lens_radius;
	u0 = orig.u0;
	u0 = orig.u0;
	u1 = orig.u1;
	v0 = orig.v0;
	v1 = orig.v1;
	d = orig.d;
}

Camera::Camera(Vector3 c, Vector3 gaze, Vector3 vup, RFLOAT aperture, RFLOAT left, RFLOAT right, RFLOAT bottom, RFLOAT top, RFLOAT distance)
	:center(c),d(distance),u0(left),u1(right),v0(bottom),v1(top)
{
	lens_radius = aperture / 2.0f;
	uvw.init_from_wv(-gaze, vup);
	corner = center + u0 * uvw.u() + v0 * uvw.v() - d * uvw.w();
	across = (u0 - u1) * uvw.u();
	up = (v0 - v1) * uvw.v();
}

Ray Camera::getRay(RFLOAT a, RFLOAT b, RFLOAT xi1, RFLOAT xi2)
{
	Vector3 origin = center + 2.0f*(xi1 - 0.5f) * lens_radius * uvw.u() + 2.0f * (xi2 - 0.5f) * lens_radius * uvw.v();
	Vector3 target = corner + across * a + up * b;
	return Ray(origin, normalize(target - origin));
}
