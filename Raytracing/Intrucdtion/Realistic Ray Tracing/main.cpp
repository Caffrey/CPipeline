#include "Shape.h"
#include "Image.h"
#include <vector>
using namespace std;


int main()
{
	HitRecord rec;
	bool is_a_hit;

	RFLOAT tmax;
	Vector dir(0, 0, -1);

	vector<Shape*> shapes;
	shapes.push_back(new Sphere(Vector(250, 250, -1000), 150, Color(.2, .2, .8)));
	shapes.push_back(
		new Triangle(
			Vector(300.0f, 600.0f, -800),
			Vector(0.0f, 100.0f, -1000),
			Vector(450.0f, 20.0f, -1000),
			Color(.8, .2, .2)));

	Image im(500, 500);

	for (int i = 0; i < 500; i++)
	{
		for (int j = 0; j < 500; j++)
		{
			tmax = 10000.0f;
			is_a_hit = false;
			Ray r(Vector(i, j, 0), dir);

			for (int k = 0; k < shapes.size(); k++)
			{
				if (shapes[k]->hit(r, .00001f, tmax,rec))
				{
					tmax = rec.t;
					is_a_hit = true;
				
			}
			}
			if (is_a_hit)
				im.set(i, j, rec.color);
			else
				im.set(i, j, Color(.2, .2, .2));
		}
	}
	im.writePPM2(cout);


	return -1;
}