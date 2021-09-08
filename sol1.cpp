
#include<bits/stdc++.h>
using namespace std;



 string dup = "";
 vector<vector<string>> ans;


bool isPalindrome(string s)
{
    int n = s.size();
     int i =0,j = n-1;
      
       while(i<=j)
       {
           if(s[i] != s[j])return false;
           dup += s[i];
           i++;
           j--;
           
       }
       return true;
}


  void solve(int i , string s,vector<string> temp)
    {
        if(i >= s.size()) 
        {
           
            // int k = temp.size();
            // for(auto i : temp)
            // {
            //     cout << i << " ";
            // }


            ans.push_back(temp);
            return;
        }
        
        
        for(int j =i;j<s.size();j++)
        {
            string left = s.substr(i,j-i + 1);
            if(isPalindrome(left))
            {
                temp.push_back(left);
                solve(j + 1,s,temp);
                temp.pop_back();
            }
        }
        
        
    }    
    



int main() 
{


int t;
cin >> t;
while(t--)
{

   dup = "";
  
   string s;
   cin >> s;
   int n = s.size();
   if(isPalindrome(s))
   {
      
       cout << dup << "\n";
       
    
   }
   else
   {
       vector<string> temp;
        solve(0,s,temp);
        cout << "[";

        for(int i =0;i<ans.size();i++)
        {
            cout << "[";
            for(int j = 0;j<ans[i].size();j++)
            {
                 if(j == ans[i].size() - 1)
                 {
                     cout << ans[i][j] ;
                 }
                 else
                 {
                     cout << ans[i][j] << ", " ;
                 }


            }
           if(i ==ans.size() - 1)
           {
               cout << "]";
           }
           else
           {
                cout << "],";
           }
        }
        cout << "]";
        
        cout << "\n";

   }

}

	return 0;
}


