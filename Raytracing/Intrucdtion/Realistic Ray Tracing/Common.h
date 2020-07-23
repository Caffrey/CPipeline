#pragma once

#include <math.h>
#include <float.h>
typedef float RFLOAT;

typedef unsigned char t_u8;
typedef signed char t_s8;
typedef unsigned short t_u16;
typedef signed short t_s16;
typedef unsigned int t_u32;
typedef signed int t_s32;
typedef unsigned long long t_u64;
typedef signed long long t_s64;


#define PI 3.14159265359f
#define TWO_PI 6.28318530718f
#define HALF_PI 1.570796326795f

#define FLOAT_EPSILON 1.0e-5f

template<typename T>
void swap(T& t1, T& t2)
{
	T tmp = t1;
	t1 = t2;
	t2 = tmp;
}

template<typename T>
T clamp(T f, T min, T max)
{
	T v = f;
	if (v > max)
		v = max;
	if (v < min)
		v = min;
	return f;
}

template<typename T>
T lerp(T s, T e, RFLOAT f)
{
	return s * (1 - f) + e * f;
}

template<typename T>
T min(T t1, T t2)
{
	return t1 < t2 ? t1 : t2;
}

template<typename T>
T max(T t1, T t2)
{
	return t1 > t2 ? t1 : t2;
}

inline RFLOAT fdet3(RFLOAT a, RFLOAT b, RFLOAT c, RFLOAT d, RFLOAT e, RFLOAT f,
	RFLOAT g, RFLOAT h, RFLOAT i)
{
	return a * e * i + d * h * c + g * b * f - g * e * c - d * b * i - a * h * f;
}

inline RFLOAT fclamp01(RFLOAT f)
{
	return clamp<RFLOAT>(f, 0.0f, 1.0f);
}

inline bool fequal(RFLOAT f1, RFLOAT f2)
{
	return fabs(f1 - f2) <= FLT_EPSILON;
}


inline RFLOAT tent_filter(RFLOAT x)
{
	if (x < 0.5f)
		return sqrt(2.0f * x) - 1.0f;
	else
		return 1.0f - sqrt(2.0f - 2.0f * x);
}


