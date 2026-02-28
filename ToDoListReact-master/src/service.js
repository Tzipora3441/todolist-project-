import axios from 'axios';

const apiUrl = "http://localhost:5070"; 
axios.defaults.baseURL = apiUrl;

export default {
  getTasks: async () => {
    const result = await axios.get(`/items`)    
    return result.data;
  },

  addTask: async(name)=>{
   
  // אנחנו שולחים אובייקט, כי השרת מצפה ל-Item
  const response = await axios.post(`/items`, { 
    name: name, 
    isComplete: false 
  });
  return response.data;}
,
  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    const response=await axios.put(`/items/${id}`,
     { 
    
   
    isComplete: isComplete 
     }
      
   )
    return response.data;
  },

  deleteTask:async(id)=>{
    const res=await axios.delete(`/items/${id}`);
    console.log('deleteTask')
     return res.data;
  }
};


// ה-Interceptor: ה"שומר" של התגובות
axios.interceptors.response.use(
  // פונקציה ראשונה: מה לעשות כשהכל עבר בשלום (סטטוס 200-299)
  (response) => {
    return response; // פשוט מחזירים את התגובה כפי שהיא
  },
  
  // פונקציה שנייה: מה לעשות כשחזרה שגיאה (400, 500 וכו')
  (error) => {
    // אנחנו תופסים את השגיאה "בדרך" ורושמים אותה ללוג
    console.error('--- שגיאת API התגלתה! ---');
    console.error('נתיב:', error.config.url);
    console.error('סטטוס:', error.response?.status);
    console.error('הודעה:', error.message);
    
    // חשוב להחזיר את השגיאה בסוף כדי שהקוד שקרא לפונקציה ידע שמשהו השתבש
    return Promise.reject(error);
  }
);
