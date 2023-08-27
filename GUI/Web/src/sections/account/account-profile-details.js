import { useCallback, useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Divider,
  TextField,
  Unstable_Grid2 as Grid
} from '@mui/material';
import CountryFlag from 'country-flag-icons/react/3x2';
import countriesList from 'countries-list';
import axios from 'axios';

export const AccountProfileDetails = () => {
  const [values, setValues] = useState({});
  const [selectedCountry, setSelectedCountry] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');

  const handleFirstNameChange = useCallback((event) => {
    setFirstName(event.target.value);
  }, []);

  const handleLastNameChange = useCallback((event) => {
    setLastName(event.target.value);
  }, []);

  const handleEmailChange = useCallback((event) => {
    setEmail(event.target.value);
  }, []);

  const handlePhoneChange = useCallback((event) => {
    setPhone(event.target.value);
  }, []);

  useEffect(() => {
    axios
      .get('https://localhost:7094/GetUserByEmail')
      .then((response) => {
        console.log('data:', response.data);
        setValues(response.data.data);
        setFirstName(response.data.data.username);
        setLastName(response.data.data.lastName);
        setEmail(response.data.data.email);
        setSelectedCountry(response.data.data.country);
      })
      .catch((error) => {
        console.error('Error fetching data:', error);
      });
  }, []); 

  const handleChange = useCallback(
    (event) => {
      setValues((prevState) => ({
        ...prevState,
        [event.target.name]: event.target.value
      }));
    },
    []
  );

  const handleCountryChange = useCallback(
    (event) => {
      setSelectedCountry(event.target.value);
    },
    []
  );

  const allCountries = Object.entries(countriesList.countries).map(
    ([code, name]) => ({
      code,
      name,
    })
  );

  const handleSubmit = async() =>{
      try {
        console.log("user data: ", firstName, ' ', lastName, ' ', email, ' ', selectedCountry);
        const response = await axios.post('https://localhost:7094/UpdateUserDetails', {
          username: firstName,
          lastName,
          email : values.email,
          country: selectedCountry,
          // Add other fields as needed
        });

        // Assuming the server responds with a success message or data, you can handle it here.
        console.log('Response from server:', response.data);
        // You can also update the state or perform other actions after a successful submission.
      } catch (error) {
        // Handle any errors that occur during the request
        console.error('Error sending configuration:', error);
        // You can display an error message to the user if necessary
      }
    };


  return (
    <form
      autoComplete="off"
      noValidate
      onSubmit={handleSubmit}
    >
      <Card>
        <CardHeader
          subheader="The information can be edited"
          title="Profile"
        />
        <CardContent sx={{ pt: 0 }}>
          <Box sx={{ m: -1.5 }}>
            <Grid
              container
              spacing={3}
            >
              <Grid
                xs={12}
                md={6}
              >
                <TextField
                  fullWidth
                  helperText="Please specify the first name"
                  label="First name"
                  name="firstName"
                  onChange={handleFirstNameChange}
                  required
                  value={firstName}
                  focused 
                />
              </Grid>
              <Grid
                xs={12}
                md={6}
              >
                <TextField
                  fullWidth
                  label="Last name"
                  name="lastName"
                  onChange={handleLastNameChange}
                  value={lastName}
                  focused
                />
              </Grid>
              <Grid
                xs={12}
                md={6}
              >
                <TextField
                  fullWidth
                  label="Email Address"
                  name="email"
                  onChange={handleEmailChange}
                  required
                  value={values.email}
                  focused
                />
              </Grid>
              <Grid
                xs={12}
                md={6}
              >
                <TextField
                  fullWidth
                  label="Phone Number"
                  name="phone"
                  onChange={handlePhoneChange}
                  type="number"
                  value={phone}
                  focused
                />
              </Grid>
              <Grid
                xs={12}
                md={6}
              >
                <TextField
                  fullWidth
                  label="Country"
                  name="country"
                  onChange={handleCountryChange}
                  required
                  select
                  focused
                  SelectProps={{ native: true }}
                  value={selectedCountry != null ? selectedCountry : ''}
                >
                  {allCountries.map((country) => (
                    <option key={country.code} value={country.name.name}>
                     
                      {country.name.name ? country.name.name : ''}
                    </option>
                  ))}
                </TextField>
              </Grid>            
            </Grid>
          </Box>
        </CardContent>
        <Divider />
        <CardActions sx={{ justifyContent: 'flex-end' }}>
          <Button onClick={handleSubmit} variant="contained">
            Save details
          </Button>
        </CardActions>
      </Card>
    </form>
  );
};
