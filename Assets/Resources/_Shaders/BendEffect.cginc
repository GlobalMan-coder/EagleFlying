// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#ifndef BENDEFFECT_INCLUDED
#define BENDEFFECT_INCLUDED

uniform float _BEND_Y = 0.0f;
uniform float _BEND_X = 0.0f;
uniform float _HORIZON = 0.0f;
uniform float _SPREAD = 0.0f;

float4x4 _Camera2World;
float4x4 _World2Camera;

float4 BendEffect (float4 v)
{
	float4 t = mul (unity_ObjectToWorld, v);
	float dist = max(0, abs(_HORIZON - t.z) - _SPREAD);
	t.y -= dist * dist * _BEND_Y;
	t.x -= dist * dist * _BEND_X;
	t.xyz = mul(unity_WorldToObject, t).xyz;
	return t;
}

#endif