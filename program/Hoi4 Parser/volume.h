#ifndef _VOLUME_H
#define _VOLUME_H

#include <string>
#include "element.h"

typedef std::string Value;

// === principle of value-passing between Volume pointers ===
// if want to pass the value from a Volume pointer to another,
// use Volume::get() to pass its value to a new dynamic Volumn pointer, 
// such as
//	pNewVol = new Volume(pOld->get()) ¡Ì
// 
// other than simply pass a Volumn pointer.
//	pNewVol = pOld ¡Á
typedef struct Volume* pVolume;
struct Volume
{
private:
	pcValue const val;
	mutable bool lose_val;
private:
	// abandon to use
	Volume(const Volume& _vol) :
		val(nullptr),
		lose_val(false)
	{
	}
public:
	// will call _e->get() that transfers ownership, 
	// and will DELETE p_e and set it to nullptr 
	Volume(pElement* const p_e) :
		val((*p_e)->get()),
		lose_val(false)
	{
		delete (*p_e);
		(*p_e) = nullptr;
	}
	Volume(const Value& _v) :
		val(new std::string(_v)),
		lose_val(false)
	{
	}
	// get the ownership of _vol
	Volume(pcValue _v) :
		val(_v),
		lose_val(false)
	{
	}
	// will call _vol->get() that transfers ownership,
	// and will DELETE p_vol and set it to nullptr 
	Volume(pVolume* const p_vol) :
		val((*p_vol)->get()),
		lose_val(false)
	{
		delete (*p_vol);
		(*p_vol) = nullptr;
	}
	~Volume()
	{
		if (!lose_val) { delete val; }
	}
	const Value& value() const
	{
		return *val;
	}
	//
	// if called for any time, ownership of pointer to vol will lose, and ~Value() won't delete vol 
	//
	pcValue get() const
	{
		lose_val = true;
		return val;
	}
	const char& head()
	{
		return (*val)[0];
	}
	friend bool operator==(const Volume& lhs, const Volume& rhs)
	{
		return lhs.value() == rhs.value();
	}
	friend bool operator!=(const Volume& lhs, const Volume& rhs)
	{
		return lhs.value() != rhs.value();
	}
};

#endif // ! _VOLUME_H
