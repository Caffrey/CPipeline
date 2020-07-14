#pragma once
#include "Common.h"
// linear congruence algorithm
class Random
{
public:
	Random(t_u64 _seed = 7654321ULL)
	{
		seed = _seed;
		mult = 62089911ULL;
		u64_max = 4294967295ULL;
		flt_max = 4294967295.0f;
	}

	inline RFLOAT operator()()
	{
		seed = mult * seed;
		return RFLOAT(seed % u64_max) / flt_max;
	}

public:
	t_u64 seed;
	t_u64 mult;
	t_u64 u64_max;
	RFLOAT flt_max;
};

