export default function SignOut() {

    localStorage.removeItem('jwt');
    window.location.href = '/';


};