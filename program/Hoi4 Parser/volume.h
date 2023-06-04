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
// or pass a pointer to pVolume so that delete and set pVolume to nullptr instantly,
// 
// other than simply pass a Volumn pointer.
//	pNewVol = pOld ¡Á
namespace hoi4
{
	namespace parser
	{
		struct Volume
		{
		private:
			const pcval_u val;
			mutable bool own_val;
		public:
			// will call (*p_e)->get() that transfers ownership of value, 
			// and will DELETE (*p_e) and set it to nullptr 
			Volume(Element& _e) :
				val(_e.get()),
				own_val(true)
			{
			}
			// get the ownership of value
			Volume(pcval_u _v) :
				val(_v),
				own_val(true)
			{
			}
			// will call (*p_vol)->get() that transfers ownership of value,
			// and will DELETE (*p_vol) and set it to nullptr 
			Volume(const Volume& _vol) :
				val(_vol.get()),
				own_val(true)
			{
			}
			~Volume()
			{
				if (own_val) { delete val; }
			}
			pcval_u value() const
			{
				return val;
			}
			//
			// if called for any time, ownership of value will lose, that ~pcval_u() won't delete it 
			//
			pcval_u get() const
			{
				own_val = false;
				return val;
			}
			bool equals(Volume* _vol)
			{
				return strcmp(val, _vol->value()) == 0 ? true : false;
			}
			operator bool()
			{
				return own_val;
			}
		};
	}
}
#endif // ! _VOLUME_H
