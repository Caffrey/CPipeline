#pragma once
#include "Common.h"
#include "Vector.h"
#include "ortho_basis.h"
class Ray;
class Vector3;
class ONB;
class Camera
{
public:
	Camera();
	Camera(const Camera& orig);
	Camera(Vector3 c, Vector3 gaze, Vector3 vup, RFLOAT aperture, RFLOAT left, RFLOAT right, RFLOAT bottom, RFLOAT top,
		RFLOAT distance);

	Ray getRay(RFLOAT a, RFLOAT b, RFLOAT xi1, RFLOAT xi2);

	Vector3 center,corner,across,up;
	ONB uvw;
	RFLOAT lens_radius;
	RFLOAT u0, u1, v0, v1;
	RFLOAT d;
	

};

