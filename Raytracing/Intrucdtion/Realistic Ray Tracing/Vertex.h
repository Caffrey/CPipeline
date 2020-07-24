#pragma once
#include "Vector.h"

struct VertexUV
{
	Vector3 vertex;
	Vector2 uv;
};

struct VertexN
{
	Vector3 vertex;
	Vector3 normal;
};

struct VertexUVN
{
	Vector3 vertex;
	Vector3 normal;
	Vector2 uv;

};