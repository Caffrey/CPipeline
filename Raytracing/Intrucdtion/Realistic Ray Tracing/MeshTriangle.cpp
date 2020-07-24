#include "MeshTriangle.h"
#include "Mesh.h"
#include "Common.h"


MeshTriangleUV::MeshTriangleUV()
{

}

MeshTriangleUV::MeshTriangleUV(Mesh* _mesh, int p0, int p1, int p2, int index)
	:mesh_ptr(_mesh)
{
	p[0] = p0;
	p[1] = p1;
	p[2] = p2;

}

MeshTriangleUV::~MeshTriangleUV() {}

bool MeshTriangleUV::hit(const Ray& r, float tmin, float tmax, HitRecord& record) const
{
	Vector3 p0((mesh_ptr->vertUVs[p[0]]).vertex);
	Vector3 p1((mesh_ptr->vertUVs[p[1]]).vertex);
	Vector3 p2((mesh_ptr->vertUVs[p[2]]).vertex);

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

	if (beta <= 0.0f || beta >= 1.0f)	return false;

	RFLOAT AKJB = A * K - J * B;
	RFLOAT JCAL = J * C - A * L;
	RFLOAT BLKC = B * L - K * C;

	RFLOAT gamma = (I * AKJB + H * JCAL + G * BLKC) / denom;
	if (gamma <= 0.0f || beta + gamma >= 1.0f)	return false;

	tval = -(F * AKJB + E * JCAL + D * BLKC) / denom;
	if (tval >= tmin && tval <= tmax)
	{
		double alpha = 1.0 - beta - gamma;
		Vector2 u0((mesh_ptr->vertUVs[p[0]]).uv);
		Vector2 u1((mesh_ptr->vertUVs[p[1]]).uv);
		Vector2 u2((mesh_ptr->vertUVs[p[2]]).uv);
	
		record.uv = Vector2(alpha * u0.x() + beta * u1.x() + gamma * u2.x(),
			alpha * u0.y() + beta * u1.y() + gamma * u2.y());

		record.hit_tex = mesh_ptr->getTexture();
		record.t = tval;
		record.normal = normalize(cross(p1 - p0, p2 - p0));
		return true;
	
	}
	return false;

}

bool MeshTriangleUV::shadowHit(const Ray& r, float tmin, float tmax, HitRecord& record) const
{
	Vector3 p0((mesh_ptr->vertUVs[p[0]]).vertex);
	Vector3 p1((mesh_ptr->vertUVs[p[1]]).vertex);
	Vector3 p2((mesh_ptr->vertUVs[p[2]]).vertex);

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

	if (beta <= 0.0f || beta >= 1.0f)	return false;

	RFLOAT AKJB = A * K - J * B;
	RFLOAT JCAL = J * C - A * L;
	RFLOAT BLKC = B * L - K * C;

	RFLOAT gamma = (I * AKJB + H * JCAL + G * BLKC) / denom;
	if (gamma <= 0.0f || beta + gamma >= 1.0f)	return false;

	tval = -(F * AKJB + E * JCAL + D * BLKC) / denom;

	return (tval >= tmin && tval <= tmax);
}
