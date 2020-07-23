#include "Sample.h"
#include "Vector.h"
#include <stdlib.h>
#include "common_math.h"

void random(Vector2* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{		
		
		samples[i].set_x(drand48());
		samples[i].set_y(drand48());
	}
}

void jitter(Vector2* samples, int num_samples)
{
	int sqrt_samples = (int)(sqrt(num_samples));
	double double_sqrt_samples = (double)sqrt_samples;
	for (int i = 0; i < sqrt_samples; i++)
	{
		for (int j = 0; j < sqrt_samples; j++)
		{
			RFLOAT x = ((double)i + drand48()) / double_sqrt_samples;
			RFLOAT y = ((double)j + drand48()) / double_sqrt_samples;
			(samples[i * sqrt_samples + j]).set_x(x);
			(samples[i * sqrt_samples + j]).set_x(y);
		}
	}
}

void nrooks(Vector2* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{
		samples[i].set_x(((double)i + drand48()) / (double)num_samples);
		samples[i].set_y(((double)i + drand48()) / (double)num_samples);
	}
	for (int i = num_samples-2; i >=0 ;i--)
	{
		int target = int(drand48() * (double)i);
		RFLOAT temp = samples[i + 1].x();
		samples[i + 1].set_x(samples[target].x());
		samples[target].set_x(temp);
	}

}

// assues num_samples is a perfect square
void multiJitter(Vector2* samples, int num_samples)
{
	int sqrt_samples = (int)sqrt(num_samples);
	RFLOAT subcell_width = 1.0 / (float(num_samples));

	for(int i = 0; i < sqrt_samples; i++)
		for (int j = 0; j < sqrt_samples; j++)
		{
			samples[i * sqrt_samples + j][0] = i * sqrt_samples * subcell_width +
				j * subcell_width + drand48() * subcell_width;
			
			samples[i * sqrt_samples + j][1] = j * sqrt_samples * subcell_width +
				i * subcell_width + drand48() * subcell_width;
		}

	//shuffle x coords within each colum and y coords within each row
	for(int i = 0; i <sqrt_samples;i++)
		for (int j = 0; j < sqrt_samples; j++)
		{
			int k = j + int(drand48() * (sqrt_samples - j - 1));
			RFLOAT t = samples[i * sqrt_samples + j][0];
			samples[i * sqrt_samples + j][0] = samples[i * sqrt_samples + k][0];
			samples[i * sqrt_samples + k][0] = t;

			k = j + int(drand48() * (sqrt_samples - j - 1));
			t = samples[j * sqrt_samples + i][1];
			
			samples[j * sqrt_samples + i][1] = samples[k * sqrt_samples + i][1];
			samples[k * sqrt_samples + i][1] = t;
		}
}

void shuffle(Vector2* samples, int num_samples)
{
	for (int i = num_samples - 2; i >= 0; i--)
	{
		int target = int(drand48() * (double)i);
		Vector2 temp = samples[i + 1];
		samples[i + 1] = samples[target];
		samples[target] = temp;
	}
}


//1D sampling

void random(RFLOAT* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{
		samples[i] = drand48();
	}
}

void jitter(RFLOAT* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{
		samples[i] = ((double)i + drand48()) / (double)num_samples;
	}
}

void shuffle(RFLOAT* samples, int num_samples)
{
	for (int i = num_samples - 2; i >= 0; i--)
	{
		int target = int(drand48() * (double)i);
		RFLOAT temp = samples[i + 1];
		samples[i + 1] = samples[target];
		samples[target] = temp;
	}

}

void boxFilter(Vector2* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{
		samples[i].set_x(samples[i].x() - 0.5f);
		samples[i].set_y(samples[i].y() - 0.5f);
	}
}

void tenFilter(Vector2* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{
		RFLOAT x = samples[i].x();
		RFLOAT y = samples[i].y();

		if (x < 0.5f) 
		{
			samples[i].set_x((RFLOAT)sqrt(2.0 * (double)x) - 1.0f);
		}
		else
		{
			samples[i].set_x(1.0f - (RFLOAT)sqrt(2.0 - 2.0 * (double)x));
		}
		if (y < 0.f)
		{
			samples[i].set_y((RFLOAT)sqrt(2.0 * (double)y) - 1.0f);
		}
		else
		{
			samples[i].set_y(1.0f - (RFLOAT)sqrt(2.0 - 2.0 * (double)y));
		}
	}
}

void cubicSplineFilter(Vector2* samples, int num_samples)
{
	for (int i = 0; i < num_samples; i++)
	{
		RFLOAT x = samples[i].x();
		RFLOAT y = samples[i].y();

		samples[i][0] = cubicFilter(x);
		samples[i][1] = cubicFilter(y);
	}
}
