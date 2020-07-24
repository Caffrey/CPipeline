#pragma once
#include "Shape.h"

class Mesh;
class Texture;

class MeshTriangleUV : public Shape
{
	MeshTriangleUV();
	MeshTriangleUV(Mesh* _mesh, int p0, int p1, int p2, int index);
	~MeshTriangleUV();
	 bool hit(const Ray& r, float tmin, float tmax, HitRecord& record) const = 0;
	 bool shadowHit(const Ray& r, float tmin, float tmax, HitRecord& record) const = 0;

	 int p[3];
	Mesh* mesh_ptr;
};



