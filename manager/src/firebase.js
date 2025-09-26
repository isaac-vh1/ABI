// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyB9bZ91dIsqqIcb3W5QKqSwgTooRRAd6XY",
  authDomain: "abi-management.firebaseapp.com",
  projectId: "abi-management",
  storageBucket: "abi-management.firebasestorage.app",
  messagingSenderId: "549679174966",
  appId: "1:549679174966:web:a09e9a14a57e3e770b4f97",
  measurementId: "G-LWL1XKB42X"
};

// Initialize Firebase
console.log("Loaded Firebase API Key:", process.env.REACT_APP_FIREBASE_API_KEY);
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);