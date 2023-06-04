#ifndef _ELEMENT_H
#define _ELEMENT_H

#include <memory>

namespace hoi4
{
	namespace parser
	{
		// remember pcval_u should always be a new char[]
		typedef std::unique_ptr<const char[]> pcval_u;

		//typedef struct Element* pElement;
		struct Element
		{
		private:
			size_t ln;
			size_t col;
			pcval_u val;
			bool own_val;
		public:
			Element() :
				own_val(false),
				ln(0),
				col(0),
				val(nullptr)
			{
			}
			// will call (*p_e)->get() that transfers ownership of value, 
			// and will DELETE (*p_e) and set it to nullptr 
			Element(Element& _e) :
				own_val(true),
				ln(_e.line()),
				col(_e.column()),
				val(_e.get()) // same as above ^
			{
			}
			~Element()
			{
				//if (own_val) { delete[] val; }
			}
			void operator()(char ch, const size_t& line, const size_t& column)
			{
				ln = line;
				//if (own_val) { delete val; }
				//else { own_val = true; }
				char* buf = new char[] { ch, '\0' };
				val.reset();
				val = pcval_u(buf);
				col = column;

			}
			void operator()(pcval_u str, size_t strlen, const size_t& line, const size_t& end_column)
			{
				ln = line;
				if (own_val) { delete val; }
				else { own_val = true; }
				char* buf = new char[strlen + 1];
				for (size_t i = 0; i < strlen; i++)
				{
					buf[i] = str[i];
				}
				buf[strlen] = '\0';
				val = pcval_u(buf);
				col = end_column - strlen;

			}
			const size_t& line()
			{
				return ln;
			}
			const size_t& column()
			{
				return col;
			}
			// the first char to compare with 
			const char& head()
			{
				return val[0];
			}
			// if called for any time, ownership of pointer to val will lose, and ~Element() won't delete[] val
			pcval_u get()
			{
				own_val = false;
				return val;
			}
			operator bool()
			{
				return own_val;
			}
		};
	}
}

#endif // !_ELEMENT_H

