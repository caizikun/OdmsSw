#!/usr/bin/python
# -*- coding: UTF-8 -*-
#Jun-Seok Choi


from __future__ import division
import pylab
from scipy.optimize import curve_fit
import numpy as np
import os, sys, glob 

def MUX_fitting(fn, fitting_range_nm = 10) :
	
	print "Processing " + fn 
	
	## ============ read file ====================
	wls, db1s, db2s, db3s, db4s = [], [], [], [], []
		
	lines = open(fn, 'r').readlines()
	
	for line in lines:
		
		if line.strip() == '' : continue
		
		tmp = [float(x) for x in line.strip().split(',')]
	
		wls.append(tmp[0])
		db1s.append(tmp[1])
		db2s.append(tmp[2])
		db3s.append(tmp[3])
		db4s.append(tmp[4])
	
	## ============ sorting with max value ============================
	
	if db1s.index(max(db1s)) > db4s.index(max(db4s)) :  ## need reverse sorting
		db1s, db2s, db3s, db4s = db4s, db3s, db2s, db1s 	
	
	## ============ move file to old/ ================
	dir_old = os.path.split(fn)[0] + '/' + 'old'
	try:	os.mkdir(dir_old)
	except: pass
	
	## ============ data slicing ================
	tmpwl1s = []
	tmpwl2s = []
	tmpwl3s = []
	tmpwl4s = []
			
	tmpdb1s = []
	tmpdb2s = []
	tmpdb3s = []
	tmpdb4s = []
			
	for enum, wl in enumerate(wls):
		
		if wl > 1271 - fitting_range_nm and wl < 1271 + fitting_range_nm :			
			tmpwl1s.append(wl)
			tmpdb1s.append(db1s[enum])
			
		if wl > 1291 - fitting_range_nm and wl < 1291 + fitting_range_nm :
			tmpwl2s.append(wl)
			tmpdb2s.append(db2s[enum])
			
		if wl > 1311 - fitting_range_nm and wl < 1311 + fitting_range_nm :
			tmpwl3s.append(wl)
			tmpdb3s.append(db3s[enum])
			
		if wl > 1331 - fitting_range_nm and wl < 1331 + fitting_range_nm :
			tmpwl4s.append(wl)
			tmpdb4s.append(db4s[enum])
	
	
	## ============ fitting ===================
	
	def fitfunc(wl, a, b, c):
		return a * ( wl + b )**2 + c


	X, Y = curve_fit(fitfunc, tmpwl1s, tmpdb1s, p0=(-1, -1270, -3))	
	a1, b1, c1 = X
		
	X, Y = curve_fit(fitfunc, tmpwl2s, tmpdb2s, p0=(-1, -1290, -3))	
	a2, b2, c2 = X
	
	X, Y = curve_fit(fitfunc, tmpwl3s, tmpdb3s, p0=(-1, -1310, -3))	
	a3, b3, c3 = X
	
	X, Y = curve_fit(fitfunc, tmpwl4s, tmpdb4s, p0=(-1, -1330, -3))	
	a4, b4, c4 = X
	
	fit_tmpdb1s = [a1 * (x + b1)**2 + c1 for x in tmpwl1s]
	fit_tmpdb2s = [a2 * (x + b2)**2 + c2 for x in tmpwl2s]
	fit_tmpdb3s = [a3 * (x + b3)**2 + c3 for x in tmpwl3s]
	fit_tmpdb4s = [a4 * (x + b4)**2 + c4 for x in tmpwl4s]
	
	
	## ============ inserting fit data to original data =================
	
	new_db1s, new_db2s, new_db3s, new_db4s = db1s[:], db2s[:], db3s[:], db4s[:]
	
	for enum, wl in enumerate(wls):
		
		if wl > 1271 - fitting_range_nm and wl < 1271 + fitting_range_nm :
			new_db1s[enum] = a1 * (wl + b1)**2 + c1 
			
		if wl > 1291 - fitting_range_nm and wl < 1291 + fitting_range_nm :
			new_db2s[enum] = a2 * (wl + b2)**2 + c2 
	
		if wl > 1311 - fitting_range_nm and wl < 1311 + fitting_range_nm :
			new_db3s[enum] = a3 * (wl + b3)**2 + c3 
			
		if wl > 1331 - fitting_range_nm and wl < 1331 + fitting_range_nm :
			new_db4s[enum] = a4 * (wl + b4)**2 + c4 
			
	## ============ writing to new file =================
	os.rename(fn, dir_old + '/' + os.path.basename(fn))
	f = open(fn, 'w')
	
	for enum, wl in enumerate(wls):
		tmpstr = str(wl) + ',' + format(new_db1s[enum], "0.3f") + ',' + format(new_db2s[enum], "0.3f") + ',' + format(new_db3s[enum], "0.3f") + ',' + format(new_db4s[enum], "0.3f") + '\n'
		f.write(tmpstr)
	
	f.close()
	
	if False:		
		pylab.figure()
		
		pylab.plot(wls, db1s, ':')
		pylab.plot(wls, db2s, ':')
		pylab.plot(wls, db3s, ':')
		pylab.plot(wls, db4s, ':')
		
		pylab.show()
		

		pylab.figure()
		
		pylab.plot(wls, new_db1s)
		pylab.plot(wls, new_db2s)
		pylab.plot(wls, new_db3s)
		pylab.plot(wls, new_db4s)
		
		pylab.show()
	
	print "Finishing " + fn 
	return 

#fns = glob.glob("*.txt")
#for fn in fns :
#	MUX_fitting(fn)

MUX_fitting(sys.argv[1])
