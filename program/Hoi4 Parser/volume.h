#ifndef _VOLUME_H
#define _VOLUME_H

#include <string>

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
	Volume(const std::string* _vol) :
		vol(_vol),
		lose_vol(false)
	{
	}
	// will call _vol.get() that transfers ownership
	Volume(const Volume& _vol) :
		vol(_vol.get()),
		lose_vol(false)
	{
	}
	// will call _vol->get() that transfers ownership
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
