// test virtual.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <memory>
#include <vector>

using namespace std;
class A;
class B;
class Base
{
protected:
    enum Type
    {
        TypeA,
        TypeB
    } type;
public:
    ~Base() { cout << "del base"; }
public:
    Base(Type _t, int a) 
    { 
        type = _t;
        cout << a << endl;
    }
    virtual Base* append(Base* base)const { return nullptr; };
    Type get_type() { return type; }
protected:
     int test(int a) 
    { 
        return a; 
    }
};
class A : public Base
{
public:
    mutable int a = 1;
    struct S
    {
        char a;
        int b;
        S()
        {
            a = 100;
            b = 200;
        }
    } s;
    A() : Base(TypeA, mul(5)) {}
    A* append(A* base) const
    { 
        if (base->get_type() != TypeA)
        {
            cout << "A cant append no typeA\n";
        }
        auto _s = ((A*)base)->s;
        cout << _s.a << ' ' << _s.b;
    }
    void append(B* base)const
    {
        int c = 0;
        c = 2;
    }
    int mul(int b)
    {
        return b * 100;
    }
    void test()
    {
        cout << "A";
    }
    vector<
};
class B : public Base
{
public:
    B() : Base(TypeB, 10) {}
    B* append(B* base)const { cout << "B::append()\n"; }
    void append() const { cout << "B::append()\n"; }
};

int main()
{
    unique_ptr<A> a(new A);
    unique_ptr<A> b(a);
    a->test();
    b->test();
    /*char a = 't';
    string s(&a);
    s.clear();
    cout << a;*/
    //unique_ptr<string> b(new string(_a));

    /*const int& _a = 1;
    const char* ch = new char[6] {'t', 'e', 0, 's', 't', 0};
    string a = string(ch);
    a[2] = '\0';
    char* c = new char[a.length() + 10] {0};
    for (int i = 0; i < a.length(); i++)
    {
        c[i] = a[i];
    }
    const char* const p = c;
    a.clear();
    cout << *p;
    delete[] p;
    cout << p;*/
    //Base * a = new A();
    //B* b = new B();
    //Base* _b = (Base*)b;
    //const Base  & __b = *b;
    //auto c = nullptr;
    //bool test = a == c;
    //a->append(b);
    //_b->append(a);
    //b->append();
    //__b.append(a);
    //delete a;
    ////delete b;
    //delete &__b;
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
