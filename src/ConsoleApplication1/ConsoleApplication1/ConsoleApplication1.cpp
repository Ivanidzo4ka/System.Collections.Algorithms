// ConsoleApplication1.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <algorithm>
#include <utility>

struct item {
	int key, prior;
	item* l, * r;
	item() { }
	item(int key, int prior) : key(key), prior(prior), l(NULL), r(NULL) { }
};
typedef item* pitem;

void split(pitem t, int key, pitem& l, pitem& r) {
	if (!t)
		l = r = NULL;
	else if (key < t->key)
		split(t->l, key, l, t->l), r = t;
	else
		split(t->r, key, t->r, r), l = t;
}

void insert(pitem& t, pitem it) {
	if (!t)
		t = it;
	else if (it->prior > t->prior)
		split(t, it->key, it->l, it->r), t = it;
	else
		insert(it->key < t->key ? t->l : t->r, it);
}

void merge(pitem& t, pitem l, pitem r) {
	if (!l || !r)
		t = l ? l : r;
	else if (l->prior > r->prior)
		merge(l->r, l->r, r), t = l;
	else
		merge(r->l, l, r->l), t = r;
}

void erase(pitem& t, int key) {
	if (t->key == key)
		merge(t, t->l, t->r);
	else
		erase(key < t->key ? t->l : t->r, key);
}

pitem unite(pitem l, pitem r) {
	if (!l || !r)  return l ? l : r;
	if (l->prior < r->prior)
	{
		pitem temp = l;
		l = r;
		r = temp;
	}
	pitem lt, rt;
	split(r, l->key, lt, rt);
	l->l = unite(l->l, lt);
	l->r = unite(l->r, rt);
	return l;
}

pitem root;
pitem add;
int main()
{
	std::cout << "Hello World!\n";

	add = new item(1, 100);
	insert(root, add);

	add = new item(1, 300);
	insert(root, add);

	add = new item(2,200);
	insert(root, add);

	add = new item(2, 300);
	insert(root, add);

	add = new item(3, 700);
	insert(root, add);
	add = new item(3, 150);
	insert(root, add);
}