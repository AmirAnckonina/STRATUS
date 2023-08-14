import Head from 'next/head';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { useFormik } from 'formik';
import { useState } from 'react';
import * as Yup from 'yup';
import { Box, Button, Link, Stack, TextField, Typography,Select, MenuItem } from '@mui/material';
import { useAuth } from 'src/hooks/use-auth';
import { Layout as AuthLayout } from 'src/layouts/auth/layout';
import { tr } from 'date-fns/locale';
import { set } from 'nprogress';

const Page = () => {
  const router = useRouter();
  const auth = useAuth();
  const [responseMessage, setResponseMessage] = useState(null);
  const [isSuccess, setIsSuccess] = useState(false);
  const [selectedRegion, setSelectedRegion] = useState('us-east-1');
  const awsRegions = [
    { value: 'us-east-1', label: 'US East (N. Virginia)' },
    { value: 'us-east-2', label: 'US East (Ohio)' },
    { value: 'us-west-1', label: 'US West (N. California)' },
    { value: 'us-west-2', label: 'US West (Oregon)' },
    { value: 'af-south-1', label: 'Africa (Cape Town)' },
    { value: 'ap-east-1', label: 'Asia Pacific (Hong Kong)' },
    { value: 'ap-south-1', label: 'Asia Pacific (Mumbai)' },
    { value: 'ap-northeast-1', label: 'Asia Pacific (Tokyo)' },
    { value: 'ap-northeast-2', label: 'Asia Pacific (Seoul)' },
    { value: 'ap-northeast-3', label: 'Asia Pacific (Osaka-Local)' },
    { value: 'ap-southeast-1', label: 'Asia Pacific (Singapore)' },
    { value: 'ap-southeast-2', label: 'Asia Pacific (Sydney)' },
    { value: 'ca-central-1', label: 'Canada (Central)' },
    { value: 'eu-central-1', label: 'Europe (Frankfurt)' },
    { value: 'eu-west-1', label: 'Europe (Ireland)' },
    { value: 'eu-west-2', label: 'Europe (London)' },
    { value: 'eu-south-1', label: 'Europe (Milan)' },
    { value: 'eu-west-3', label: 'Europe (Paris)' },
    { value: 'eu-north-1', label: 'Europe (Stockholm)' },
    { value: 'me-south-1', label: 'Middle East (Bahrain)' },
    { value: 'sa-east-1', label: 'South America (São Paulo)' },
  ];

  const formik = useFormik({
    initialValues: {
      email: '',
      name: '',
      password: '',
      accessKey: '',
      secretKey: '',
      awsRegion: '',
      submit: null
    },
    validationSchema: Yup.object({
      email: Yup
        .string()
        .email('Must be a valid email')
        .max(255)
        .required('Email is required'),
      name: Yup
        .string()
        .max(255)
        .required('Name is required'),
      password: Yup
        .string()
        .max(255)
        .required('Password is required'),
      accessKey: Yup
        .string()
        .max(255)
        .required('Access Key is required'),
      secretKey: Yup
        .string()
        .max(255)
        .required('Secret Key is required')
    }),
    onSubmit: async (values, helpers) => {
      try {
        await auth.signUp(values.email, values.name, values.password, values.accessKey, values.secretKey, values.awsRegion);
        helpers.setStatus({ success: true });
        setIsSuccess(true);
        setResponseMessage('User created successfully');
        router.push('/'); 
      } catch (err) {
        // Handle other errors
        console.log(err.message);
        helpers.setStatus({ success: false });
        setResponseMessage(err.message);
        helpers.setSubmitting(false);
      }
    }
  });

  return (
    <>
      <Head>
        <title>
          Register | STARTUS
        </title>
      </Head>
      <Box
        sx={{
          flex: '1 1 auto',
          alignItems: 'center',
          display: 'flex',
          justifyContent: 'center'
        }}
      >
        <Box
          sx={{
            maxWidth: 550,
            px: 3,
            py: '100px',
            width: '100%'
          }}
        >
          <div>
            <Stack
              spacing={1}
              sx={{ mb: 3 }}
            >
              <Typography variant="h4">
                Register
              </Typography>
              <Typography
                color="text.secondary"
                variant="body2"
              >
                Already have an account?
                &nbsp;
                <Link
                  component={NextLink}
                  href="/auth/login"
                  underline="hover"
                  variant="subtitle2"
                >
                  Log in
                </Link>
              </Typography>
            </Stack>
            <form
              noValidate
              onSubmit={formik.handleSubmit}
            >
              <Stack spacing={4}>
                <TextField
                  error={!!(formik.touched.name && formik.errors.name)}
                  fullWidth
                  helperText={formik.touched.name && formik.errors.name}
                  label="Name"
                  name="name"
                  onBlur={formik.handleBlur}
                  onChange={formik.handleChange}
                  value={formik.values.name}
                />
                <TextField
                  error={!!(formik.touched.email && formik.errors.email)}
                  fullWidth
                  helperText={formik.touched.email && formik.errors.email}
                  label="Email Address"
                  name="email"
                  onBlur={formik.handleBlur}
                  onChange={formik.handleChange}
                  type="email"
                  value={formik.values.email}
                />
                <TextField
                  error={!!(formik.touched.password && formik.errors.password)}
                  fullWidth
                  helperText={formik.touched.password && formik.errors.password}
                  label="Password"
                  name="password"
                  onBlur={formik.handleBlur}
                  onChange={formik.handleChange}
                  type="password"
                  value={formik.values.password}
                />
                <TextField
                  error={!!(formik.touched.accessKey && formik.errors.accessKey)}
                  fullWidth
                  helperText={formik.touched.accessKey && formik.errors.accessKey}
                  label="Access Key"
                  name="accessKey"
                  onBlur={formik.handleBlur}
                  onChange={formik.handleChange}
                  value={formik.values.accessKey}
                />
                <TextField
                  error={!!(formik.touched.secretKey && formik.errors.secretKey)}
                  fullWidth
                  helperText={formik.touched.secretKey && formik.errors.secretKey}
                  label="Secret Key"
                  name="secretKey"
                  onBlur={formik.handleBlur}
                  onChange={formik.handleChange}
                  value={formik.values.secretKey}
                />
                <Select
                  fullWidth
                  label="AWS Region"
                  name="awsRegion"
                  onBlur={formik.handleBlur}
                  onChange={(event) => {
                    formik.handleChange(event);
                    setSelectedRegion(event.target.value);
                  }}
                  value={selectedRegion}
                >
                  {awsRegions.map((region) => (
                    <MenuItem key={region.value} value={region.value}>
                      {region.label}
                    </MenuItem>
                  ))}           
                </Select>
              </Stack>
              {formik.errors.submit && (
                <Typography
                  color="error"
                  sx={{ mt: 3 }}
                  variant="body2"
                >
                  {formik.errors.submit}
                </Typography>
              )}
              {responseMessage && (
                <Typography
                  color={isSuccess ? 'green' :  'red' }
                  sx={{ mt: 3 }}
                  variant="body2"
                >
                  {responseMessage}
                </Typography>
              )}
              <Button
                fullWidth
                size="large"
                sx={{ mt: 3 }}
                type="submit"
                variant="contained"
              >
                Continue
              </Button>
            </form>
          </div>
        </Box>
      </Box>
    </>
  );
};

Page.getLayout = (page) => (
  <AuthLayout>
    {page}
  </AuthLayout>
);

export default Page;
