#ifndef _VOLUME_H
#define _VOLUME_H

#include <string>
#include "element.h"

// === principle of value-passing between Volume pointers ===
// if want to pass the value from a Volume pointer to another,
// use Volume::get() to pass its value to a new dynamic Volumn pointer, 
// such as
//	pNewVol = new Volume(pOld->get()) ¡Ì
// 
// other than simply pass a Volumn pointer.
//	pNewVol = pOld ¡Á
struct Volume
{
private:
	const std::string* const vol;
	mutable bool lose_vol;
public:
	// will call _e->get() that transfers ownership, 
	// and will DELETE _e 
	Volume(Element** const p_e) :
		vol((*p_e)->get()),
		lose_vol(false)
	{
		delete (*p_e);
		(*p_e) = nullptr;
	}
	// get the ownership of _vol
	Volume(const std::string* _v) :
		vol(_v),
		lose_vol(false)
	{
	}
	// will call _vol.get() that transfers ownership,
	// and WON'T delete _vol
	Volume(const Volume& _vol) :
		vol(_vol.get()),
		lose_vol(false)
	{
	}
	// will call _vol->get() that transfers ownership,
	// and WON'T delete _vol
	Volume(const Volume* _vol) :
		vol(_vol->get()),
		lose_vol(false)
	{
	}
	~Volume()
	{
		if (!lose_vol) { delete vol; }
	}
	const std::string& volumn() const
	{
		return *vol;
	}
	//
	// if called for any time, ownership of pointer to vol will lose, and ~Value() won't delete vol
	//
	const std::string* get() const
	{
		lose_vol = true;
		return vol;
	}
	friend bool operator==(const Volume& lhs, const Volume& rhs)
	{
		return lhs.volumn() == rhs.volumn();
	}
	friend bool operator!=(const Volume& lhs, const Volume& rhs)
	{
		return lhs.volumn() != rhs.volumn();
	}
};

#endif // ! _VOLUME_H
