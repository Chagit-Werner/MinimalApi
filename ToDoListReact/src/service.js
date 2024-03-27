

import axios from 'axios';

axios.defaults.baseURL  = "https://localhost:7169"
axios.interceptors.response.use(
  function (response) {
    return response;
  },
  error => {
    console.error('Request failed:', error);
    return Promise.reject(error);
  }
);

const Tasks={
  getTasks: async () => {
    try {
      const result = await axios.get("/tasks");
       console.log(result.data);
      return result.data;
    } catch (error) {
      console.error('Error getting tasks:', error);
      throw error;
    }
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    try {
      const result = await axios.post("/tasks",{name});
      return result.data;
    } catch (error) {
      console.error('Error adding tasks:', error);
      throw error;
    }
  },

   setCompleted : async (id, isComplete, name) => {
    console.log('setCompleted', { id, isComplete });
    try {
   
      const result = await axios.put(`/tasks/${id}`, { isComplete, name });
      return result.data;
    } catch (error) {
      console.error('Error setting task completion status:', error);
      throw error;
    }
  },

  deleteTask:async(id)=>{
    console.log('deleteTask')
    try {
      await axios.delete(`/tasks/${id}`);
    } catch (error) {
      console.error('Error delete tasks:', error);
      throw error;
    }
  }
};
export default Tasks;