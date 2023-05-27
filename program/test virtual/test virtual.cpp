// test virtual.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>

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
    Base(Type _t) 
    { 
        type = _t;
    }
    virtual void append(Base* base) = 0;
    Type get_type() { return type; }
};
class A : public Base
{
public:
    int a = 1;
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
    A() : Base(TypeA) {}
    void append(Base* base) 
    { 
        if (base->get_type() != TypeA)
        {
            cout << "A cant append no typeA\n";
        }
        auto _s = ((A*)base)->s;
        cout << _s.a << ' ' << _s.b;
    }
    void append(B* base)
    {
        int c = 0;
        c = 2;
    }
};
class B : public Base
{
public:
    B() : Base(TypeB) {}
    void append(Base* base) { append(); }
    void append() { cout << "B::append()\n"; }
};

int main()
{
    Base * a = new A();
    B* b = new B();
    Base* _b = (Base*)b;
    a->append(b);
    _b->append(a);
    b->append();
    delete a;
    delete b;

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
