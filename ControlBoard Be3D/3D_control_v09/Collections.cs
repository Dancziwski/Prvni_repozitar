using System;
using Microsoft.SPOT;
using System.Collections;

namespace _3D_control_v09
{
    class Collections
    {

        /**
 * @param {Array} arr
 * @param {number} indexA
 * @param {number} indexB
 */
    private static void swap(ref ArrayList arr,int indexA, int indexB ) 
    {
	    var tmp = arr[indexA];
	    arr[indexA] = arr[indexB];
	    arr[indexB] = tmp;
    }

/**
 * @param {Array} arr
 * @param {number} start
 * @param {number} end
 * @param {function} compare
 * @return {number}
 */
    private static int partition(ref ArrayList arr,int start, int end) 
    {
	    string pivot = arr[(int)(System.Math.Floor(( start + end )/2))].ToString();
	    var i = start;
	    var j = end;

	while( i <= j ) {
        while (compareStrings((string)arr[i], pivot) < 0)
        {
			i++;
		}

        while (compareStrings((string)arr[j], pivot) > 0)
        {
			j--;
		}

		if ( i <= j ) {
			swap(ref arr, i, j );
			i++;
			j--;
		}
	}

	return i;
}

/**
 * @param {Array} arr
 * @param {number|function(a,b)} start
 * @param {number=} end
 * @param {function(a,b)=} compare // function return -1,0,1
 * @return {Array}
 */
    public static void quickSort(ref ArrayList arr,int start,int end) 
    {
        int index = 0;

	    if( arr.Count > 1 ) 
        {
		    index = partition(ref arr, start, end);

		    if( start < index - 1) {
		    	quickSort(ref arr, start, index - 1);
		    }

		if( index < end ) {
			quickSort(ref arr, index, end);
		}
	}

	//return arr;
}

/**
 * @param {string} a
 * @param {string} b
 * @retrun {number}
 */
    private static int compareStrings(string a,string b ) {
	    var l = a.Length < b.Length ? a.Length : b.Length;

	    a = a.ToLower();
	    b = b.ToLower();

	    if(a == b) 
            return 0;

	    for(var i = 0; i < l; i++) {
		    if( a[i] < b[i]) 
            {
		    	return -1;
		    } 
            else 
                if ( a[i] > b[i] ) {
			        return 1;
		    }
	    }

	    if(a.Length < b.Length) {
		    return -1;
	    } else {
		    return 1;
	    }
    }


    }
}
