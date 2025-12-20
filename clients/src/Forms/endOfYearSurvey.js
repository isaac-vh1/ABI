import React from 'react';
import { useForm } from 'react-hook-form';

export default function App() {
  const { register, handleSubmit, formState: { errors } } = useForm();
  const onSubmit = data => console.log(data);
  console.log(errors);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input type="text" placeholder="First name" {...register("First name", {required: true, maxLength: 80})} />
      <input type="text" placeholder="Email" {...register("Email", {required: true, pattern: /^\S+@\S+$/i})} />
      <input type="tel" placeholder="Mobile number" {...register("Mobile number", {required: true, minLength: 6, maxLength: 12})} />
      <select {...register("Title", { required: true })}>
        <option value="Mr">Mr</option>
        <option value="Mrs">Mrs</option>
        <option value="Miss">Miss</option>
        <option value="Dr">Dr</option>
      </select>
      <select {...register("Which services did you use this year?", { required: true })}>
        <option value="Lawn Mowing/Edging">Lawn Mowing/Edging</option>
        <option value="Trimming/Hedging">Trimming/Hedging</option>
        <option value="Seasonal Cleanup">Seasonal Cleanup</option>
        <option value="Mulch/Barking">Mulch/Barking</option>
        <option value="Weeding">Weeding</option>
        <option value="Other">Other</option>
      </select>

      <input type="submit" />
    </form>
  );
}